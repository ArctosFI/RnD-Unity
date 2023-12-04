using UnityEngine;
using OpenCvSharp;

public class ImageMarkerRecognition : MonoBehaviour
{
    public Texture2D customMarkerTexture1;
    public Texture2D customMarkerTexture2;

    private WebCamTexture webcamTexture;
    private Mat frameMat;

    // Placeholder threshold value, adjust accordingly
    private float threshold = 0.1f;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        // Adjust the initialization of frameMat to match the channels of customMarkerTexture1
        frameMat = new Mat(webcamTexture.height, webcamTexture.width, MatType.CV_8UC4);
    }

    void Update()
    {
        frameMat = OpenCvSharp.Unity.TextureToMat(webcamTexture);

        // Convert Texture2D to Mat for custom marker images
        Mat markerImage1 = TextureToMat(customMarkerTexture1);
        Mat markerImage2 = TextureToMat(customMarkerTexture2);

        // Debug log to check descriptor types
        Debug.Log("markerImage1 type: " + markerImage1.Type());
        Debug.Log("frameMat type: " + frameMat.Type());

        // Debug log to check descriptor dimensionality
        Debug.Log("markerImage1 cols: " + markerImage1.Cols);
        Debug.Log("frameMat cols: " + frameMat.Cols);

        // Ensure that both frameMat and markerImage1 have the same number of channels
        if (frameMat.Channels() != markerImage1.Channels())
        {
            Cv2.CvtColor(frameMat, frameMat, ColorConversionCodes.BGRA2BGR);
        }

        // Convert both frameMat and markerImage1 to grayscale
        Cv2.CvtColor(frameMat, frameMat, ColorConversionCodes.BGR2GRAY);
        Cv2.CvtColor(markerImage1, markerImage1, ColorConversionCodes.BGR2GRAY);

        // Resize markerImage1 to match the size of frameMat
        Cv2.Resize(markerImage1, markerImage1, new Size(frameMat.Cols, frameMat.Rows));

        // Detect custom image markers
        var detector = new BFMatcher(NormTypes.Hamming, false);
        var matches1 = detector.KnnMatch(markerImage1, frameMat, k: 2);
        var matches2 = detector.KnnMatch(markerImage2, frameMat, k: 2);

        // You need to implement logic to decide if a marker is detected based on matches
        // ...

        if (MarkerDetected(matches1) || MarkerDetected(matches2))
        {
            Debug.Log("Marker Recognized!");
        }

        // Display the processed image in Unity
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(frameMat);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    bool MarkerDetected(DMatch[][] matches)
    {
        // You need to implement logic to decide if a marker is detected based on matches
        // For example, you could check the number of good matches or use a threshold
        return matches[0][0].Distance < threshold;
    }

    Mat TextureToMat(Texture2D texture)
    {
        byte[] bytes = texture.GetRawTextureData();
        Mat mat = new Mat(texture.height, texture.width, MatType.CV_8UC4);
        mat.SetArray(0, 0, bytes);
        return mat;
    }
}
