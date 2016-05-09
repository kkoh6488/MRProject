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
    public int port = 8080;
    string uri;
    Uri queryUri, submitUri, knowledgeUri;
    string _lastEncoded;

    // State variables
    
    void Awake() {
        imgCapture = new ImageCapture();
        uri = string.Format("http://{0}:{1}", serverIp, port);
        queryUri = new Uri(uri + "/query");
        submitUri = new Uri(uri + "/submit");
        knowledgeUri = new Uri(uri + "/knowledge");
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
    }

    #region Inspector API

    public override void CaptureImage()
    {
        SubmitImageQuery();
    }

    #endregion

    #region Query functions

    /// <summary>
    /// Captures an image and sends it to the query server.
    /// </summary>
    /// <returns>True, if the image was captured successfully.</returns>
    private bool SubmitImageQuery() {
        bool result = imgCapture.StoreScreenshotBuffer(Camera.main);
        if (result)
        {
            _lastEncoded = imgCapture.GetScreenshotBufferToJPGBase64();
            string response = SendCapturedImageQuery(_lastEncoded);
            ShowQueryIndicator();
            currentState = AppState.QUERYING;
        }
        return result;
    }

    /// <summary>
    /// Sends a query request to the image search server.
    /// </summary>
    /// <param name="encodedImg">A base-64 encoded string of the query image.</param>
    /// <returns>A string response from the server.</returns>
    private string SendCapturedImageQuery(string encodedImg)
    {
        // Send packet to server containing jpeg bytes - to be reassembled on other side.
        Debug.Log("Sending img query");
        Debug.Log(encodedImg);
        string json = "{\"photorequest\": {\"name\": \"none\", \"imageData\": \"" + encodedImg +"\"}}";
        Debug.Log("Json: " + json);
        WebClient client = new WebClient();
        client.UploadStringCompleted += new UploadStringCompletedEventHandler(QueryCompleteCallback);
        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        client.UploadStringAsync(queryUri, "POST", json);
        //string result = client.UploadString(queryUri, "POST", json);
        //Debug.Log(result);
        //return result;
        return "";
    }

    private void QueryCompleteCallback(object sender, UploadStringCompletedEventArgs e)
    {
        string response = e.Result;
        Debug.Log("Got response back " + response);
        if (response.Equals("[]"))
        {
            Debug.Log("No results found.");
            SendCapturedImagePostRequest(_lastEncoded, "test");
        }
    }

    /// <summary>
    /// Submits an image to be indexed by the server.
    /// </summary>
    /// <param name="encodedImg">The image to be indexed, encoded as a base 64 string.</param>
    /// <param name="name">The label to attach to this image.</param>
    private void SendCapturedImagePostRequest(string encodedImg, string name)
    {
        string json = "{\"photorequest\": {\"name\": \"" + name + "\", \"imageData\": \"" + encodedImg + "\"}}";
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //string result = client.UploadString(submitUri, "POST", json);
        client.UploadStringCompleted += new UploadStringCompletedEventHandler(SubmitCompleteCallback);
        client.UploadStringAsync(submitUri, "POST", json);
    }

    private void SubmitCompleteCallback(object sender, UploadStringCompletedEventArgs e)
    {
        string response = e.Result;
        Debug.Log("Post result: " + response);
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
        result.SetField("Field1", "Some value1");
        result.SetField("This is field 2", "value2");
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
