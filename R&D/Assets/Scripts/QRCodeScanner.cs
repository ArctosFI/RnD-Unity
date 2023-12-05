using UnityEngine;
using System.Collections;
using ZXing;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class QRCodeScanner : NetworkBehaviour
{
    private WebCamTexture webCamTexture;
    private Rect screenRect;

    CardSpawner cs;

    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        // Check if the device has a camera
        if (WebCamTexture.devices.Length > 0)
        {
            // Use the first available camera
            webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No camera found on the device.");
        }

        SceneManager.sceneLoaded += InitCardSpawner;
    }

    void Update()
    {
        if (!IsOwner) { return; }

        Debug.Log((webCamTexture != null).ToString() + "," + webCamTexture.isPlaying);

        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            // Convert Color32 array to grayscale byte array
            byte[] grayscaleBytes = GetGrayscaleBytes(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
            Debug.Log("Update 1");
            // Use BarcodeReader on the grayscale byte array
            LuminanceSource luminanceSource = new RGBLuminanceSource(grayscaleBytes, webCamTexture.width, webCamTexture.height);
            Debug.Log("Update 2");
            BarcodeReaderGeneric barcodeReader = new BarcodeReaderGeneric();
            Debug.Log("Update 3");
            Result result = barcodeReader.Decode(luminanceSource);

            Debug.Log("Update 4");

            if (result != null)
            {
                Debug.Log("QR Recognized");
                cs.AddCardServerRPC(int.Parse(result.Text));
            }
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

    void InitCardSpawner(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Server") { cs = GameObject.Find("Game Manager").GetComponent<CardSpawner>(); }
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
}
