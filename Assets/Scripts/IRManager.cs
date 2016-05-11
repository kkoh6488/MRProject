using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public enum AppState
{
    IDLE = 0,
    QUERY = 1,
    SUBMITCONFIRM = 2,
    SUBMIT = 3,
    ERROR = 4,
}

/// <summary>
/// Manager class to handle joining of application components together.
/// </summary>
public class IRManager : IRManagerBase {

    IImageCapture imgCapture;

    public GameObject queryProgressInd;
    public GameObject alertMsg;
    public GameObject submitInput;
    public ContentManager content;

    private string _alertMsg = "";
    private bool _isAlertShown = false;
    private float _alertTimer = 3f;
    private float _elapsedTime;
    private Queue<string> _alerts;

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
        _alerts = new Queue<string>();
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
        HandleStateWindows();
        #endif
    }

    void HandleStateWindows()
    {
        switch (currentState)
        {
            case AppState.QUERY:
                HideObject(submitInput);
                ShowObject(queryProgressInd);
                break;
            case AppState.SUBMIT:
                //HideObject(submitConfirmation);
                break;
            case AppState.SUBMITCONFIRM:
                HideObject(alertMsg);
                HideObject(queryProgressInd);
                ShowObject(submitInput);
                break;
            case AppState.IDLE:
                HideObject(queryProgressInd);
                HideObject(submitInput);
                HandleAlerts();
                break;
            case AppState.ERROR:
                HideObject(queryProgressInd);
                OverrideAlert(_alertMsg);
                break;
        }
    }

    void HandleAlerts()
    {
        if (_isAlertShown)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _alertTimer)
            {
                HideObject(alertMsg);
                _isAlertShown = false;
            }
        }
        else if (!_isAlertShown && _alerts.Count > 0)
        {
            ShowAlert(_alerts.Dequeue());
            _isAlertShown = true;
            _elapsedTime = 0;
        }
    }

    void OverrideAlert(string msg)
    {
        if (_alerts.Count > 0)
        {
            _alerts.Clear();
            _elapsedTime = 0;
        }
        ShowAlert(msg);
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _alertTimer)
        {
            currentState = AppState.IDLE;
            HideObject(alertMsg);
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
        currentState = AppState.QUERY;
        SendCapturedImagePostRequest(_lastEncoded, objectName);
        ShowAlert("Indexing...");
    }

    public void CancelSubmitRequest()
    {
        currentState = AppState.IDLE;
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
            _alerts.Enqueue("An error occurred getting the screenshot.");
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
        string response = e.Result;
        if (e.Result == null)
        {
            _alertMsg = "The server could not be contacted";
            currentState = AppState.ERROR;
            return;
        }
        try
        {
            if (response.Equals("[]"))
            {
                Debug.Log("No results found.");
                currentState = AppState.SUBMITCONFIRM;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace);
        }
    }

    private void ShowObject(GameObject go)
    {
        go.SetActive(true);
    }

    private void HideObject(GameObject go)
    {
        go.SetActive(false);
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
        if (response.Contains("The server has encountered an error."))
        {
            _alertMsg = response;
            currentState = AppState.ERROR;
            return;
        }
        Debug.Log("Post result: " + response);
    }

    private void TestFakeReceivedData()
    {
        currentState = AppState.IDLE;

        // Unpack the response packet

        // If no matches
        HideObject(queryProgressInd);

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

    private void ShowAlert(string msg)
    {
        _isAlertShown = true;
        alertMsg.SetActive(true);
        alertMsg.GetComponent<Text>().text = msg;
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
