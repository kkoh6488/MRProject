using UnityEngine;
using UnityEngine.UI;

public class WindowManager : AppMonoBehaviour {

    public GameObject queryProgressInd;
    public GameObject alertMsg;
    public GameObject submitInput;
    public ContentManager content;

    private string _alertMsg = "";
    private bool _isAlertShown = false;
    private float _alertTimer = 3f;
    private float _elapsedTime;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        HandleStateWindows();
        HandleAlerts();
    }

    private void HandleStateWindows()
    {
        switch (currentState)
        {
            case AppState.QUERY:
                HideObject(submitInput);
                ShowObject(queryProgressInd);
                break;
            case AppState.SUBMIT:
                break;
            case AppState.SUBMITCONFIRM:
                HideObject(alertMsg);
                HideObject(queryProgressInd);
                ShowObject(submitInput);
                break;
            case AppState.IDLE:
                HideObject(queryProgressInd);
                HideObject(submitInput);
                break;
            case AppState.ERROR:
                HideObject(queryProgressInd);
                break;
        }
    }

    private void HandleAlerts()
    {
        if (_isAlertShown)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _alertTimer)
            {
                HideObject(alertMsg);
            }
        }
        else if (!_isAlertShown && !_alertMsg.Equals(""))
        {
            ShowAlert(_alertMsg);
        }
    }

    public void SetAlert(string msg)
    {
        _alertMsg = msg;
        _isAlertShown = false;
        _elapsedTime = 0;
    }

    private void ShowAlert(string msg)
    {
        _isAlertShown = true;
        alertMsg.SetActive(true);
        alertMsg.GetComponent<Text>().text = msg;
    }

    private void ShowObject(GameObject go)
    {
        go.SetActive(true);
    }

    private void HideObject(GameObject go)
    {
        go.SetActive(false);
    }
}
