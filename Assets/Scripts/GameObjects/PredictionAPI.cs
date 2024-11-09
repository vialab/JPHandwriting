using System.Collections;
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

    private bool _online = debugMode;
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
        using UnityWebRequest webRequest = UnityWebRequest.Get(healthCheckURL);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success) {
            HealthCheckResult result = JsonUtility.FromJson<HealthCheckResult>(webRequest.downloadHandler.text);

            if (result.status.Equals("OK")) {
                LogEvent("API up, client can send requests");
                yield break;
            } 
        } 
        
        // Theoretically should only be reachable if not successful
        debugMode = true;
        LogEvent("API offline, entering debug mode");
    }

    private IEnumerator MakeAPIRequest(VocabItem vocabItem, byte[] image) {
        if (debugMode) {
            string nextChar = vocabItem.LetterAtCurrentPosition;
            
            EventBus.Instance.OnLetterPredicted.Invoke(vocabItem, nextChar);
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

                // send back to current item
                EventBus.Instance.OnLetterPredicted.Invoke(vocabItem, result.prediction);
                
                // remove prediction lock
                _predictionLock = false;
                break;
            }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError: {
                Debug.LogError($"{webRequest.result}: {webRequest.error}");
            break;
            }
        }
    }

    public void LogEvent(string message) {
       EventBus.Instance.OnLoggableEvent.Invoke(this, message); 
    }
}
