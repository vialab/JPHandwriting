using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PredictionResult {
    public string Prediction;
    public string Romaji;
}
public class PredictionAPI : EventSubscriber, ILoggable, OnLetterExported.IHandler {
    [SerializeField] private string url = "http://localhost:5000";
    [SerializeField] private string endpoint = "predict";

    private string endpointURL => $"{url}/{endpoint}";
    
    private bool _predictionLock = false;
    
    protected override void Start() {
        base.Start();
        LogEvent("Prediction request sender active");
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

    private IEnumerator MakeAPIRequest(VocabItem vocabItem, byte[] image) {
        WWWForm form = new WWWForm();
        form.AddBinaryData("img", image, "image.png");
        
        using UnityWebRequest webRequest = UnityWebRequest.Post(endpointURL, form);

        yield return webRequest.SendWebRequest();
        switch (webRequest.result) {
            case UnityWebRequest.Result.Success: {
                var resultText = webRequest.downloadHandler.text;
                PredictionResult result = JsonUtility.FromJson<PredictionResult>(resultText);
                
                LogEvent($"Prediction received: {result.Prediction} ({result.Romaji})");

                // send back to current item
                EventBus.Instance.OnLetterPredicted.Invoke(vocabItem, result.Prediction);
                
                // remove prediction lock
                _predictionLock = false;
                break;
            }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError: {
                Debug.LogError(webRequest.result);
            break;
            }
        }
    }

    public void LogEvent(string message) {
       EventBus.Instance.OnLoggableEvent.Invoke(this, message); 
    }
}
