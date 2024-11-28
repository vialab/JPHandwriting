using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class ImageExporter : EventSubscriber, ILoggable, OnLetterWritten.IHandler {
    protected override void Start() {
        base.Start();
    }

    void OnLetterWritten.IHandler.OnEvent(VocabItem vocabItem, Object objWithRenderTexture) {
        ExportRenderTextureToImage(vocabItem, objWithRenderTexture.GetComponent<Renderer>().sharedMaterial.mainTexture);
    }
    
    private void ExportRenderTextureToImage(VocabItem vocabItem, Texture whiteboardCanvas)  {
        int imageWidth = whiteboardCanvas.width, imageHeight = whiteboardCanvas.height;

        RenderTexture resizedRenderTexture = RenderTexture.GetTemporary(imageWidth, imageHeight);
        Graphics.Blit(whiteboardCanvas, resizedRenderTexture);

        NativeArray<byte> imgBuffer = new NativeArray<byte>(imageWidth * imageHeight * 4, Allocator.Persistent,
        NativeArrayOptions.UninitializedMemory);

        var request =
        AsyncGPUReadback.RequestIntoNativeArray(ref imgBuffer, resizedRenderTexture, 0, (readbackRequest) => {
        if (!readbackRequest.hasError) {
            var encoded = ImageConversion.EncodeNativeArrayToPNG(imgBuffer, resizedRenderTexture.graphicsFormat,
                (uint)imageWidth, (uint)imageHeight);

            byte[] encoded_bytes = encoded.ToRawBytes();
            
            EventBus.Instance.OnLetterExported.Invoke(vocabItem, encoded_bytes);

            encoded.Dispose();
        }

        imgBuffer.Dispose();

        LogEvent($"No error in image export process? {!readbackRequest.hasError}"); 
        });
    }

    public void LogEvent(string message, LogLevel level = LogLevel.Info) {
        EventBus.Instance.OnLoggableEvent.Invoke(this, message, level);
    }
}
