using System;
using System.Net;
using UnityEngine;
using RESTUtils;
using SimpleJSON;

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

    public string testLabel;
    public Vector2 captureResolution = new Vector2(1280, 720);
    IImageCapture imgCapture;
    public WindowManager window;
    public ContentManager content;
	private RESTClient knowledge;
	private RESTClient query;
	private RESTClient submit;


    private IParser<QueryResult> _parser;

    // Networking variables
    public string serverIp = "127.0.0.1";
    public int port = 8080;
    string uri;
    Uri queryUri, submitUri, knowledgeUri;
    string _lastEncoded;


    // State variables
    
    void Awake() {
        ConfigureURIs();
        imgCapture = new ImageCapture((int) captureResolution.x, (int) captureResolution.y);
        _parser = new ResultParser();
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
        else if (Input.GetKeyDown(KeyCode.G))
        {
			SendKnowledgeQuery(testLabel);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            imgCapture.StoreScreenshotBuffer(Camera.main);
            _lastEncoded = imgCapture.GetScreenshotBufferToJPGBase64();
            SendSubmitQuery(testLabel);
        }
        #endif
    }

    private void ConfigureURIs()
    {
        uri = string.Format("http://{0}:{1}", serverIp, port);
        query = new RESTClient(uri + "/query");
        submit = new RESTClient(uri + "/submit");
        knowledge = new RESTClient(uri + "/knowledge");
        Debug.Log("Reconfigured URIs for : " + uri);
    }
    
    /// <summary>
    /// Public accessor for updating URIs.
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void SetServerAddress(string ip, int port)
    {
        this.serverIp = ip;
        this.port = port;
        ConfigureURIs();
    }

    #region Inspector API

    public override void CaptureImage()
    {
        SubmitImageQuery();
        window.SetAlert("Querying...");
    }

    public void SendSubmitQuery(string objectName)
    {
        currentState = AppState.QUERY;
        SendCapturedImagePostRequest(_lastEncoded, objectName);
        window.SetAlert("Indexing...");
    }

    public void CancelSubmitRequest()
    {
        currentState = AppState.IDLE;
    }

    #endregion

    #region Query functions

    /// <summary>
    /// Sends a knowledge query to the server.
    /// </summary>
    /// <param name="entity"></param>
    private void SendKnowledgeQuery(string entity)
    {
        Debug.Log("Sending knowledge query for : " + entity);      
		RESTRequest rr = new RESTRequest (HTTPMethod.GET, KnowledgeQueryCallback);
		rr.AddParameter ("entity", entity);
		StartCoroutine(knowledge.sendRequest (rr));
    }

    /// <summary>
    /// Callback for a knowledge search query.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void KnowledgeQueryCallback(String data)
    {
		Debug.Log (data);
		if (data.Contains("\"error\""))
        {
            window.SetAlert("The server encountered an error.");
            currentState = AppState.ERROR;
        }
        else
        {
            if (data.Equals(""))
            {
                currentState = AppState.ERROR;
                Debug.Log("The server could not be contacted");
                window.SetAlert("The server could not be contacted");
                return;
            }
            QueryResult[] results = _parser.ParseGroup(data);
            content.HandleResults(results);
        }
    }

    /// <summary>
    /// Captures an image and sends it to the query server.
    /// </summary>
    /// <returns>True, if the image was captured successfully.</returns>
    private void SubmitImageQuery() {
        _lastEncoded = null;
        bool result = imgCapture.StoreScreenshotBuffer(Camera.main);
        if (result)
        {
            _lastEncoded = imgCapture.GetScreenshotBufferToJPGBase64();
            SendCapturedImageQuery(_lastEncoded);
            currentState = AppState.QUERY;
        }
        else
        {
            window.SetAlert("The screenshot could not be captured.");
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
        string json = "{\"name\": \"none\", \"imageData\": \"" + encodedImg +"\"}";
		byte[] data = System.Text.UnicodeEncoding.Unicode.GetBytes (json);
        Debug.Log("Json: " + json);
		RESTRequest rr = new RESTRequest (HTTPMethod.POST, data, QueryCompleteCallback);
		StartCoroutine (query.sendRequest (rr));
//        WebClient client = new WebClient();
//        client.UploadStringCompleted += new UploadStringCompletedEventHandler(QueryCompleteCallback);
//        client.Headers[HttpRequestHeader.ContentType] = "application/json";
//        client.UploadStringAsync(queryUri, "POST", json);
        return "";
    }

	private void QueryCompleteCallback(string data)
    {
		if (data.Contains("\"error\""))
        {
            window.SetAlert("The server encountered an error");
            currentState = AppState.ERROR;
            return;
        }
        try
        {
            Debug.Log("response:" + data);
            if (data.Equals("[]"))
            {
                Debug.Log("No results found.");
                currentState = AppState.SUBMITCONFIRM;
			} else {
                if (data.Equals(""))
                {
                    window.SetAlert("The server could not be contacted");
                    currentState = AppState.ERROR;
                }
                else
                {
				    QueryResult[] results = _parser.ParseGroup(data);
				    content.HandleResults(results);
				    currentState = AppState.IDLE;
                }
			}
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace);
        }
    }

    /// <summary>
    /// Submits an image to be indexed by the server.
    /// </summary>
    /// <param name="encodedImg">The image to be indexed, encoded as a base 64 string.</param>
    /// <param name="name">The label to attach to this image.</param>
    private void SendCapturedImagePostRequest(string encodedImg, string name)
    {
        string json = "{\"name\": \"" + name + "\", \"imageData\": \"" + encodedImg + "\"}";
		byte[] data = System.Text.UnicodeEncoding.Unicode.GetBytes (json);

		RESTRequest rr = new RESTRequest (HTTPMethod.POST, data, SubmitCompleteCallback);
		StartCoroutine (submit.sendRequest (rr));
//
//        WebClient client = new WebClient();
//        client.Headers[HttpRequestHeader.ContentType] = "application/json";
//        client.UploadStringCompleted += new UploadStringCompletedEventHandler(SubmitCompleteCallback);
//        client.UploadStringAsync(submitUri, "POST", json);
    }

    /// <summary>
    /// Callback for completion of an image index submit request.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void SubmitCompleteCallback(string data)
    {
		Debug.Log ("Data = " + data);
        currentState = AppState.IDLE;
        string response = data;
        if (response.Contains("The server has encountered an error."))
        {
            window.SetAlert(response);
            currentState = AppState.ERROR;
            return;
        }
        Debug.Log("Post result: " + response);
        window.SetAlert(response);
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
