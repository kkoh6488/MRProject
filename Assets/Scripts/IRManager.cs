using System;
using UnityEngine;

/// <summary>
/// Manager class to handle joining of application components together.
/// </summary>
public class IRManager : IRManagerBase {

    IImageCapture imgCapture;

    void Awake() {
        imgCapture = new ImageCapture();
    }

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            imgCapture.StoreScreenshotBuffer(Camera.main);
        }
        #endif
    }

    /// <summary>
    /// Captures an image and stores it temporarily for saving if needed.
    /// </summary>
    /// <returns>True, if the image was captured successfully.</returns>
    public override bool CaptureImage() {
        return imgCapture.StoreScreenshotBuffer(Camera.main);
    }

    /// <summary>
    /// Saves the stored image to file.
    /// </summary>
    public override void SaveImage(string filename) {
        if (filename.Equals("Enter filename"))
        {
            DateTime now = DateTime.Now;
            filename = String.Format("{0}-{1}-{2}-{3}{4}{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }
        imgCapture.SaveScreenshot(filename);
    }
}
