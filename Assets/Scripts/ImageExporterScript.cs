using System;
using Unity.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Networking;

// https://gist.github.com/asus4/a19118e04a0682d65cffe5c08911a498
public static class NativeArrayExtension {
    public static byte[] ToRawBytes<T>(this NativeArray<T> arr) where T : struct {
        var slice = new NativeSlice<T>(arr).SliceConvert<byte>();
        var bytes = new byte[slice.Length];
        slice.CopyTo(bytes);
        return bytes;
    }

    public static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes) where T : struct {
        var byteArr = new NativeArray<byte>(bytes, Allocator.Temp);
        var slice = new NativeSlice<byte>(byteArr).SliceConvert<T>();

        UnityEngine.Debug.Assert(arr.Length == slice.Length);
        slice.CopyTo(arr);
    }
}

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
    // for best results, will only need to be PNG
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
                    
                    PredictionControllerScript.instance.PredictLetter(encoded.ToRawBytes());
                    
                    encoded.Dispose();
                }

                imgBuffer.Dispose();
                
                Debug.Log(!readbackRequest.hasError);
            });
    }
}