using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PredictionResult {
    public string prediction;
    public string romaji;
}

public class PredictionControllerScript : MonoBehaviour {
    [SerializeField] private bool debug = true;
    
    public static PredictionControllerScript instance;

    private bool _isPredicting = false;
    
    // Singleton should totally know what word is being guessed at any time
    // This is not bad architecture 
    private _BaseVocabItem _focusedVocabItem;
    public _BaseVocabItem FocusedVocabItem {
        get => _focusedVocabItem;
        set => _focusedVocabItem = value;
    }
    
    private string url = "http://localhost:5000";
    private byte[] _img;

    private void Awake() {
        instance ??= this;
        
    }

    public void PredictLetter(byte[] img) {
        // Read image
        ActivityLogger.Instance.LogEvent("Sending prediction API request", this);
        
        // Lock so it doesn't happen multiple times ; w; 
        if (_isPredicting) {
            ActivityLogger.Instance.LogEvent("Request cannot be made as another request is already in progress", this);
            return;
        }
        
        // I hope nothing breaks
        _img = img;
        _isPredicting = true;
        if (!debug) {
            StartCoroutine(MakeAPIRequest());
        } 
    }

    private IEnumerator MakeAPIRequest() {
        WWWForm form = new WWWForm();
        
        form.AddBinaryData("img", _img, "image.png");
        
        using UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

        yield return webRequest.SendWebRequest();

        switch (webRequest.result) {
            case UnityWebRequest.Result.Success: {
                var resultText = webRequest.downloadHandler.text;

                PredictionResult result = JsonUtility.FromJson<PredictionResult>(resultText);
                
                ActivityLogger.Instance.LogEvent($"Prediction received: {result.prediction} ({result.romaji})", this);

                // send back to current item
                _focusedVocabItem.WritingStateGameObject.AddLetter(result.prediction);
                _isPredicting = false;
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
}
