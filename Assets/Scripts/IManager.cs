/// <summary>
/// Interface for the application manager.
/// </summary>
public interface IManager {

    /// <summary>
    /// Captures an image as defined in this application.
    /// </summary>
    void CaptureImage();

    /// <summary>
    /// Saves the captured image to a file.
    /// </summary>
    /// <param name="filename">The name of the file to output to.</param>
    void SaveImage(string filename);
}
