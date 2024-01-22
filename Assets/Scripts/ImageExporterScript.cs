using System;
using Unity.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

public class ImageExporterScript : MonoBehaviour {
    [SerializeField] private RenderTexture whiteboardCanvas;
    
    private void Start() {
        WhiteboardScript.OnHandwritingFinish += Whiteboard_OnHandwritingFinish;
    }

    private void Whiteboard_OnHandwritingFinish (object sender, EventArgs e) {
        ExportRenderTextureToImage();
    }

    // I can get away with hardcoding things
    // will be as big as the canvas, only limited to B/W
    // will only need to be PNG
    private void ExportRenderTextureToImage() {
        int imageWidth = whiteboardCanvas.width, imageHeight = whiteboardCanvas.height;
        
        RenderTexture resizedRenderTexture = RenderTexture.GetTemporary(imageWidth, imageHeight);
        Graphics.Blit(whiteboardCanvas, resizedRenderTexture);

        NativeArray<byte> imgBuffer = new NativeArray<byte>(imageWidth * imageHeight * 4, Allocator.Persistent,
            NativeArrayOptions.UninitializedMemory);

        var request =
            AsyncGPUReadback.RequestIntoNativeArray(ref imgBuffer, resizedRenderTexture, 0, (readbackRequest) => {
                if (!readbackRequest.hasError) {
                    NativeArray<byte> encoded;

                    encoded = ImageConversion.EncodeNativeArrayToPNG(imgBuffer, resizedRenderTexture.graphicsFormat,
                        (uint)imageWidth, (uint)imageHeight);
                    System.IO.File.WriteAllBytes("test.png", encoded.ToArray());
                    encoded.Dispose();
                }

                imgBuffer.Dispose();
                
                Debug.Log(!readbackRequest.hasError);
            });
    }
}