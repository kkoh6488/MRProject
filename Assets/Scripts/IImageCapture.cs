using UnityEngine;

/// <summary>
/// Interface for the image capture component.
/// </summary>
public interface IImageCapture
{
    bool StoreScreenshotBuffer(Camera cam);

    void SaveScreenshot(string filename);
}
