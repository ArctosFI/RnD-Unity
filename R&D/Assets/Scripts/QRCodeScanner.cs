using UnityEngine;
using System.Collections;
using ZXing;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRCodeScanner : NetworkBehaviour
{
    private WebCamTexture webCamTexture;
    private bool shouldRestartWebCam = false;

    CardSpawner cs;

    public RawImage rawImage;

    void Start()
    {
        // Check if the device has a camera
        if (WebCamTexture.devices.Length > 0)
        {
            // Use the first available camera
            StartCoroutine(StartWebCam());
        }

        SceneManager.sceneLoaded += InitCardSpawner;
    }

    IEnumerator StartWebCam()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
            webCamTexture.Play();
            Debug.Log(webCamTexture.isPlaying);

            // Wait for the first frame to ensure that the webcam has started
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);
            Debug.Log("test");

            // Assign the webcam texture to the RawImage
            rawImage.texture = webCamTexture;

            // Start processing frames
            StartCoroutine(ProcessFrames());
        }
        else
        {
            Debug.LogError("Camera permission denied.");
        }
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

            if (result != null && cs != null)
            {
                // QR code detected, do something with the result
                Debug.Log("QR Code Detected: " + result.Text);
                cs.AddCardServerRPC(int.Parse(result.Text));

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
        if (!IsOwner) { return; }

        if (shouldRestartWebCam)
        {
            // Use a coroutine to restart the webcam
            StartCoroutine(StartWebCam());
            shouldRestartWebCam = false;
        }
    }

    void InitCardSpawner(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Server")
        { 
            cs = GameObject.Find("Game Manager").GetComponent<CardSpawner>();
            rawImage = GameObject.Find("RawImage").GetComponent<RawImage>();
            if (webCamTexture != null && webCamTexture.isPlaying) { webCamTexture.Stop(); }
            shouldRestartWebCam = true;
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
