using UnityEngine;

/// <summary>
/// Application GUI for handling input fields and image capture.
/// </summary>
public class InputGUI : MonoBehaviour {

    public IRManagerBase manager;

    // Filename input fields
    public bool isImageCaptured = false;
    private int width = 120;
    private int height = 30;
    private int maxLength = 30;
    private int submitOffset = 20;
    private int screenCenterX, screenCenterY;
    string filename = "Enter filename";

    // Capture button fields
    private int buttonSize = 60;

    void Awake() {
        screenCenterX = (Screen.width - width) / 2;
        screenCenterY = (Screen.height - height) / 2;
    }

	void OnGUI() {
        ShowCaptureButton();
        if (isImageCaptured) {
            ShowFilenameInput();
        }
    }

    /// <summary>
    /// Shows the capture button. TODO: Replace with icon.
    /// </summary>
    void ShowCaptureButton() {
        Rect captureButton = new Rect(Screen.width - buttonSize - 30, Screen.height - buttonSize - 30, buttonSize, buttonSize);
        if (GUI.Button(captureButton, "Capture"))
        {
            manager.CaptureImage();
            isImageCaptured = true;
        }
    }

    /// <summary>
    /// Displays the filename input fields at the center of the screen.
    /// </summary>
    void ShowFilenameInput() {
        Rect inputBoxPosition = new Rect(screenCenterX, screenCenterY, width, height);
        Rect submitButton = new Rect(screenCenterX + width + submitOffset, screenCenterY, width / 2, height);
        filename = GUI.TextField(inputBoxPosition, filename, maxLength);
        if (GUI.Button(submitButton, "Save"))
        {
            manager.SaveImage(filename);
            isImageCaptured = false;
            filename = "Enter filename";
        }
    }
}
