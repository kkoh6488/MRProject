using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Handles displaying received query data in the appropriate tabs and menus.
/// </summary>
public class ContentManager : AppMonoBehaviour {

    public RectTransform bodyRT;
    public Vector2 bodyDisplayPos = Vector2.zero;
    public ContentPanel googleGraphPanel;
    public Text emptyMessage;

    // State variables
    public bool isPanelShown { get; private set; }

    // Animation variables
    private Vector2 _bodyRTOffscreen;
    private bool _isSlidingIn = false;
    private bool _isSlidingOut = false;
    private float _minDelta = 0.05f;
    private Vector2 _currVel;
    private float _smoothTime = 0.3f;

    // Content size variables
    public RectTransform bodyScrollContent;
    private float _imgSizePx = 105f;
    private float _descriptionSizePx = 130f;
    private float _textRowPx = 15f;

    private QueryResult[] _lastResults;
    private bool _newResults = false;

    // Events
    public UnityEvent OnPanelShow;
    public UnityEvent OnPanelHide;

    // Use this for initialization
    void Start () {
        _bodyRTOffscreen = bodyRT.anchoredPosition;
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_lastResults == null)
        {
            emptyMessage.gameObject.SetActive(true);
            bodyScrollContent.gameObject.SetActive(false);
        }
        else
        {
            emptyMessage.gameObject.SetActive(false);
            bodyScrollContent.gameObject.SetActive(true);
        }
        if (_newResults)
        {
            DisplayMainResult(_lastResults[0]);
            SlideInResults();
            _isSlidingOut = false;
            _newResults = false;
        }
	    if (_isSlidingIn)
        {
            SlideInResults();
        }
        else if (_isSlidingOut)
        {
            SlideOutResults();
        }
	}

    public void HandleResults(QueryResult[] results)
    {
        _lastResults = results;
        _newResults = true;
    }

    private void DisplayMainResult(QueryResult q)
    {
        _isSlidingIn = true;
        bodyRT.anchoredPosition = _bodyRTOffscreen;
        AdjustContentPanelHeight(q);
        googleGraphPanel.SetDisplayContent(q);
    }

    private void AdjustContentPanelHeight(QueryResult q)
    {
        Vector2 contentSize = new Vector2(bodyScrollContent.sizeDelta.x, 0);
        if (q.HasImage())
        {
            contentSize += new Vector2(0, _imgSizePx);
        }
        if (q.HasTitle())
        {
            contentSize += new Vector2(0, _textRowPx);
        }
        if (q.HasDescription())
        {
            contentSize += new Vector2(0, _descriptionSizePx);
        }
        if (q.HasSubtitle())
        {
            contentSize += new Vector2(0, _textRowPx);
        }
        contentSize += new Vector2(0, _textRowPx * (q.fields.Count));
        bodyScrollContent.sizeDelta = contentSize;
        Debug.Log("Setting content size as " + contentSize);
    }

    private void SlideInResults()
    {
        if (Vector2.Distance(bodyRT.anchoredPosition, bodyDisplayPos) < _minDelta)
        {
            _isSlidingIn = false;
            isPanelShown = true;
            OnPanelShow.Invoke();
            return;
        }
        else
        {
            bodyRT.anchoredPosition = Vector2.SmoothDamp(bodyRT.anchoredPosition, bodyDisplayPos, ref _currVel, _smoothTime);
        }
    }

    private void SlideOutResults()
    {
        if (Vector2.Distance(bodyRT.anchoredPosition, _bodyRTOffscreen) < _minDelta)
        {
            _isSlidingOut = false;
            isPanelShown = false;
            OnPanelShow.Invoke();
            return;
        }
        else
        {
            bodyRT.anchoredPosition = Vector2.SmoothDamp(bodyRT.anchoredPosition, _bodyRTOffscreen, ref _currVel, _smoothTime);
        }
    }

    public void TogglePanel()
    {
        if (isPanelShown)
        {
            _isSlidingOut = true;
        }
        else
        {
            _isSlidingIn = true;
        }
    }
}
