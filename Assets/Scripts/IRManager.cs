using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    public int port = 8080;
    string uri;
    Socket sock;
    IPAddress serverAddr;
    IPEndPoint endPoint;

    // State variables
    

    void Awake() {
        imgCapture = new ImageCapture();
        serverAddr = IPAddress.Parse(serverIp);
        endPoint = new IPEndPoint(serverAddr, port);
        uri = string.Format("http://{0}:{1}", serverIp, port);
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
            string encoded = imgCapture.GetScreenshotBufferToJPGBase64();
            string response = SendCapturedImageQuery(encoded);
            ShowQueryIndicator();
            currentState = AppState.QUERYING;
            if (response.Equals("[]"))
            {
                Debug.Log("No results found.");
                SendCapturedImagePostRequest(encoded, "test");
            }
        }
        return result;
    }

    private string SendCapturedImageQuery(string encodedImg)
    {
        // Send packet to server containing jpeg bytes - to be reassembled on other side.
        Debug.Log("Sending img query");
        Debug.Log(encodedImg);
        string json = "{\"photorequest\": {\"name\": \"none\", \"imageData\": \"" + encodedImg +"\"}}";
        Debug.Log("Json: " + json);
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        string result = client.UploadString(uri + "/query", "POST", json);
        Debug.Log(result);
        return result;
    }

    private void SendCapturedImagePostRequest(string encodedImg, string name)
    {
        string json = "{\"photorequest\": {\"name\": \"" + name + "\", \"imageData\": \"" + encodedImg + "\"}}";
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        string result = client.UploadString(uri + "/submit", "POST", json);
        Debug.Log("Post result: " + result);
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
        string img = "https://www.google.com.au/url?sa=i&rct=j&q=&esrc=s&source=imgres&cd=&cad=rja&uact=8&ved=0ahUKEwju6LGNhczMAhUFmZQKHVRtAxsQjRwIBw&url=https%3A%2F%2Fplus.google.com%2Fu%2F0%2F116899029375914044550&psig=AFQjCNEBsosGXnX2-PkBavVBGwKnZPPCPg&ust=1462850551646464";
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
