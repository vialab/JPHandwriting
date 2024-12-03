using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class PredictionResult {
    public string prediction;
    public string romaji;
}

public class HealthCheckResult {
    public string status;
}

public class PredictionAPI : EventSubscriber, ILoggable, OnLetterExported.IHandler {
    [SerializeField] private string url = "http://localhost:5000";
    [SerializeField] private string endpoint = "predict";
    [SerializeField] private string healthEndpoint = "ping";
    [SerializeField] private static bool debugMode = false;

    private string endpointURL => $"{url}/{endpoint}";
    private string healthCheckURL => $"{url}/{healthEndpoint}";

    private bool _predictionLock = false;
    
    protected override void Start() {
        base.Start();
        LogEvent("Prediction request sender active");

        StartCoroutine(MakeHealthCheckRequest());
    }

    void OnLetterExported.IHandler.OnEvent(VocabItem vocabItem, byte[] image) {
        LogEvent("Sending prediction API request");
        
        // This is here because I once sent 500 API requests of the same one image.
        if (_predictionLock) {
            LogEvent("Request cannot be made as another request is already in progress");
            return;
        }
        
        // Fun fact: Unity added support for asynchronous functions in *2023.2*, 
        // which I'm *not* using.
        _predictionLock = true;
        StartCoroutine(MakeAPIRequest(vocabItem, image));
    }

    private IEnumerator MakeHealthCheckRequest() {
        using UnityWebRequest healthCheckRequest = UnityWebRequest.Get(healthCheckURL);

        yield return healthCheckRequest.SendWebRequest();

        if (healthCheckRequest.result == UnityWebRequest.Result.Success) {
            HealthCheckResult result = JsonUtility.FromJson<HealthCheckResult>(healthCheckRequest.downloadHandler.text);

            if (result.status.Equals("OK")) {
                LogEvent("API up, client can send requests");
            } 
        }
        else {
            // Theoretically should only be reachable if not successful
            debugMode = true;
            LogEvent("API offline, entering debug mode");
        }
    }

    private IEnumerator MakeAPIRequest(VocabItem vocabItem, byte[] image) {
        if (debugMode) {
            string nextChar = vocabItem.LetterAtCurrentPosition;
            
            LogEvent($"Debug mode, assuming character is {nextChar}");
            InvokeLetterPredicted(vocabItem, nextChar);
            yield break;
        }
        
        WWWForm form = new WWWForm();
        form.AddBinaryData("img", image, "image.png");
        
        using UnityWebRequest webRequest = UnityWebRequest.Post(endpointURL, form);

        yield return webRequest.SendWebRequest();
        switch (webRequest.result) {
            case UnityWebRequest.Result.Success: {
                var resultText = webRequest.downloadHandler.text;
                LogEvent(resultText);
                
                PredictionResult result = JsonUtility.FromJson<PredictionResult>(resultText);
                
                LogEvent($"Prediction received: {result.prediction} ({result.romaji})");
                
                InvokeLetterPredicted(vocabItem, result.prediction);
                
                break;
            }
            case UnityWebRequest.Result.ProtocolError: {
                // TODO: figure out why it's sending both GET and POST request
                break;
            }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError: {
                Debug.LogError($"{webRequest.result}: {webRequest.error}");
                break;
            }
        }
    }

    private void InvokeLetterPredicted(VocabItem vocabItem, string letterPredicted) {
        // send predicted letter
        EventBus.Instance.OnLetterPredicted.Invoke(vocabItem, letterPredicted, vocabItem.CurrentPosition);
        
        // remove prediction lock
        _predictionLock = false;
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
       EventBus.Instance.OnLoggableEvent.Invoke(this, message, level); 
    }
}
