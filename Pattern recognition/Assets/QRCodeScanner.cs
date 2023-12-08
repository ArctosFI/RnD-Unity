using UnityEngine;
using System.Collections;
using ZXing;

public class QRCodeScanner : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private bool shouldRestartWebCam = false;

    private Rect screenRect;

    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        // Check if the device has a camera
        if (WebCamTexture.devices.Length > 0)
        {
            // Use the first available camera
            StartCoroutine(StartWebCam());
        }
    }

    IEnumerator StartWebCam()
    {
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
        webCamTexture.Play();

        // Wait until the webcam has fully started
        yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

        // Start processing frames
        StartCoroutine(ProcessFrames());
    }

    IEnumerator ProcessFrames()
    {
        while (true)
        {
            // Convert Color32 array to grayscale byte array
            byte[] grayscaleBytes = GetGrayscaleBytes(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);

            // Use BarcodeReader on the grayscale byte array
            LuminanceSource luminanceSource = new RGBLuminanceSource(grayscaleBytes, webCamTexture.width, webCamTexture.height);
            BarcodeReaderGeneric barcodeReader = new BarcodeReaderGeneric();
            Result result = barcodeReader.Decode(luminanceSource);

            if (result != null)
            {
                // QR code detected, do something with the result
                Debug.Log("QR Code Detected: " + result.Text);

                // Stop the webcam when a QR code is detected
                StopWebCam();

                // Set the flag to restart the webcam
                shouldRestartWebCam = true;
            }

            yield return null;
        }
    }

    void Update()
    {
        if (shouldRestartWebCam)
        {
            // Use a coroutine to restart the webcam
            StartCoroutine(StartWebCam());
            shouldRestartWebCam = false;
        }
    }

    void OnGUI()
    {
        // Display the camera feed on the screen
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            GUI.DrawTexture(screenRect, webCamTexture, ScaleMode.ScaleToFit);
        }
    }

    private byte[] GetGrayscaleBytes(Color32[] colors, int width, int height)
    {
        byte[] grayscaleBytes = new byte[width * height];

        for (int i = 0; i < colors.Length; i++)
        {
            // Convert color to grayscale using luminance
            float luminance = colors[i].r * 0.299f + colors[i].g * 0.587f + colors[i].b * 0.114f;
            grayscaleBytes[i] = (byte)luminance;
        }

        return grayscaleBytes;
    }

    private void StopWebCam()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
