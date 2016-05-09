using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum AppState
{
    IDLE = 0,
    QUERYING = 1,
}

/// <summary>
/// Manager class to handle joining of application components together.
/// </summary>
public class IRManager : IRManagerBase {

    IImageCapture imgCapture;

    public GameObject queryProgressInd;
    public ContentManager content;

    // Networking variables
    public string serverIp = "127.0.0.1";
    public int port = 12000;
    Socket sock;
    IPAddress serverAddr;
    IPEndPoint endPoint;

    // State variables
    

    void Awake() {
        imgCapture = new ImageCapture();
        serverAddr = IPAddress.Parse(serverIp);
        endPoint = new IPEndPoint(serverAddr, port);
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
        else if (Input.GetKeyDown(KeyCode.F))
        {
            OnResultReceived();
        }
        #endif
    }

    void OnApplicationQuit()
    {
        if (sock != null)
        {
            sock.Close();
        }
    }

    #region Inspector API

    public override void CaptureImage()
    {
        SubmitImageQuery();
    }

    #endregion

    #region Query functions

    /// <summary>
    /// Captures an image and stores it temporarily for saving if needed.
    /// </summary>
    /// <returns>True, if the image was captured successfully.</returns>
    private bool SubmitImageQuery() {
        bool result = imgCapture.StoreScreenshotBuffer(Camera.main);
        if (result)
        {
            SendCapturedImageQuery(imgCapture.GetScreenshotBufferToBytes());
            ShowQueryIndicator();
            currentState = AppState.QUERYING;
        }
        return result;
    }

    private void SendCapturedImageQuery(byte[] imgBytes)
    {
        // Send packet to server containing jpeg bytes - to be reassembled on other side.
    }

    private void OnResultReceived()
    {
        currentState = AppState.IDLE;

        // Unpack the response packet

        // If no matches
        HideQueryIndicator();

        // Else
        // Make the query result
        // Fake data
        string title = "Name";
        string subtitle = "Object type";
        string description = "Some long description Some long description Some long description Some long description Some long description Some long description Some long description Some long description";
        string img = "imgurl";
        QueryResult result = new QueryResult(title, subtitle, description, img);
        content.HandleResult(result);
    }

    /// <summary>
    /// Manually label the last sent query image with specified text.
    /// </summary>
    private void LabelImage()
    {

    }

    private void ShowQueryIndicator()
    {
        queryProgressInd.SetActive(true);
    }

    private void HideQueryIndicator()
    {
        queryProgressInd.SetActive(false);
    }

    #endregion

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
