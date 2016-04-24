using UnityEngine;

/// <summary>
/// Component for displaying the hardware camera feed. Attach this to an object to set its texture to the camera.
/// </summary>
public class CameraTexture : MonoBehaviour {

    private WebCamTexture camTexture;
    private Renderer planeRenderer;
    private const int Width = 1280;
    private const int Height = 720;

    void Awake() {
        InitialiseWebcamTexture();
    }

    /// <summary>
    /// Initialises the webcam texture using the first detected camera device.
    /// </summary>
    void InitialiseWebcamTexture() {
        if (WebCamTexture.devices.Length > 0){
            string device = WebCamTexture.devices[0].name;
            camTexture = new WebCamTexture(device, Width, Height);
            planeRenderer = GetComponent<Renderer>();
            planeRenderer.material.mainTexture = camTexture;
            camTexture.Play();
        }
    }
}
