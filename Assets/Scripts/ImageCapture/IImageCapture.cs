using UnityEngine;

/// <summary>
/// Interface for the image capture component.
/// </summary>
public interface IImageCapture
{
    /// <summary>
    /// Stores the view of the given camera in a temporary screenshot buffer.
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    bool StoreScreenshotBuffer(Camera cam);

    /// <summary>
    /// Saves the current screenshot in the buffer to file.
    /// </summary>
    /// <param name="filename"></param>
    void SaveScreenshot(string filename);

    /// <summary>
    /// Retrieves the bytes for the buffered screenshot in JPG format.
    /// </summary>
    /// <returns></returns>
    byte[] GetScreenshotBufferToBytes();
}
