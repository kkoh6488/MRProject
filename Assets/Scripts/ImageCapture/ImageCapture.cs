using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Handles capture of camera render and output to the file system.
/// </summary>
public class ImageCapture : IImageCapture {

    private enum ImgFormat
    {
        JPG = 0,
        PNG = 1,
    }

    private RenderTexture rt;
    private ImgFormat format = ImgFormat.JPG;
    private const int Width = 1280;
    private const int Height = 720;
    private Texture2D tempTexture;
    private string savePath;

    public ImageCapture() {
        savePath = Application.persistentDataPath + "/";
    }

    /// <summary>
    /// Captures the image from a particular camera. 
    /// </summary>
    /// <param name="cam">The camera to render the image.</param>
    /// <param name="filename">The filename of the output file, not including the extension.</param>
    /// <param name="width">The pixel width of the saved image.</param>
    /// <param name="height">The pixel height of the saved image.</param>
    public bool StoreScreenshotBuffer(Camera cam) {
        rt = new RenderTexture(Width, Height, 24);
        cam.targetTexture = rt;
        int temp = cam.cullingMask;
        cam.cullingMask = 1 << LayerMask.NameToLayer("CameraFeed");
        cam.Render();
        tempTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        tempTexture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Debug.Log("Captured image");
        cam.cullingMask = temp;
        return true;
    }

    /// <summary>
    /// Saves the screenshot currently in the temporary texture to file.
    /// </summary>
    /// <param name="filename">The name for the saved file.</param>
    public void SaveScreenshot(string filename) {
        if (tempTexture != null) {
            SaveTextureToFile(filename, tempTexture);
            tempTexture = null;
        } else
        {
            throw new System.NullReferenceException("There is no stored image to save.");
        }
    }

    /// <summary>
    /// Saves the texture to file.
    /// </summary>
    /// <param name="filename">The name of the image file to be saved.</param>
    /// <param name="tex">The texture to use for the image.</param>
    void SaveTextureToFile(string filename, Texture2D tex) {
        byte[] bytes;
        string outputPath;
        if (format == ImgFormat.JPG)
        {
            bytes = tempTexture.EncodeToJPG();
            outputPath = savePath + filename + ".jpg";
        }
        else {
            bytes = tempTexture.EncodeToPNG();
            outputPath = savePath + filename + ".png";
        }
        File.WriteAllBytes(outputPath, bytes);
        Debug.Log("Saved to file: " + outputPath);
    }

    /// <summary>
    /// Gets the buffered screenshot in JPG format as a byte array.
    /// </summary>
    /// <returns>A byte array of the JPG screenshot.</returns>
    public byte[] GetScreenshotBufferToBytes()
    {
        if (tempTexture != null)
        {
            return tempTexture.EncodeToJPG();
        }
        return null;
    }

    /// <summary>
    /// Gets the buffered screenshot in JPG format as a base 64 encoded string.
    /// </summary>
    /// <returns>Base 64 encoding of the JPG screenshot.</returns>
    public string GetScreenshotBufferToJPGBase64()
    {
        if (tempTexture != null)
        {
            byte[] jpgBytes = tempTexture.EncodeToJPG();
            Debug.Log("Jpg bytes length: " + jpgBytes.Length);
            File.WriteAllBytes("test.jpg", jpgBytes);
            return Convert.ToBase64String(jpgBytes).Replace(" ", "");
        }
        return null;
    }
}
