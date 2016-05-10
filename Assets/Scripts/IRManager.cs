using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public enum AppState
{
    IDLE = 0,
    QUERY = 1,
    SUBMIT = 2,
}

/// <summary>
/// Manager class to handle joining of application components together.
/// </summary>
public class IRManager : IRManagerBase {

    IImageCapture imgCapture;

    public GameObject queryProgressInd;
    public GameObject alertMsg;
    public GameObject submitConfirmation;
    public ContentManager content;
    private string _alertMsg = "";
    private bool _isAlertShown = false;
    private float _alertTimer = 3f;
    private float _elapsedTime;

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
            TestFakeReceivedData();
        }
        HandleProgressIndicator();
        #endif
    }

    void HandleProgressIndicator()
    {
        if (currentState == AppState.QUERY)
        {
            ShowProgressIndicator("Searching...");
        }
        else if (currentState == AppState.SUBMIT)
        {
            ShowProgressIndicator("Indexing");
        }
        else 
        {
            HideProgressIndicator();
            HideAlert();
        }
    }

    #region Inspector API

    public override void CaptureImage()
    {
        SubmitImageQuery();
        ShowAlert("Querying...");
    }

    public void SendSubmitQuery(string objectName)
    {
        SendCapturedImagePostRequest(_lastEncoded, objectName);
        currentState = AppState.QUERY;
        ShowAlert("Indexing...");
    }

    #endregion

    #region Query functions

    /// <summary>
    /// Captures an image and sends it to the query server.
    /// </summary>
    /// <returns>True, if the image was captured successfully.</returns>
    private void SubmitImageQuery() {
        bool result = imgCapture.StoreScreenshotBuffer(Camera.main);
        if (result)
        {
            _lastEncoded = imgCapture.GetScreenshotBufferToJPGBase64();
            SendCapturedImageQuery(_lastEncoded);
            currentState = AppState.QUERY;
        }
        else
        {
            throw new Exception("An error occurred getting the screenshot.");
        }
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
        return "";
    }

    private void QueryCompleteCallback(object sender, UploadStringCompletedEventArgs e)
    {
        currentState = AppState.IDLE;
        string response = e.Result;
        if (e.Result == null)
        {
            Debug.Log("Server error");
            return;
        }
        try
        {
            if (response.Equals("[]"))
            {
                Debug.Log("No results found.");
                ConfirmSubmitQuery();
            }
            Debug.Log(response == null);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace);
        }
    }

    private void ConfirmSubmitQuery()
    {
        submitConfirmation.SetActive(true);
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
        currentState = AppState.IDLE;
        string response = e.Result;
        Debug.Log("Post result: " + response);
    }

    private void TestFakeReceivedData()
    {
        currentState = AppState.IDLE;

        // Unpack the response packet

        // If no matches
        HideProgressIndicator();

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

    private void ShowProgressIndicator(string msg)
    {
        queryProgressInd.SetActive(true);
    }

    private void HideProgressIndicator()
    {
        queryProgressInd.SetActive(false);
    }

    private void ShowAlert(string msg)
    {
        _elapsedTime = 0f;
        _isAlertShown = true;
        alertMsg.SetActive(true);
        alertMsg.GetComponent<Text>().text = msg;
    }

    private void HideAlert()
    {
        alertMsg.gameObject.SetActive(false);
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
