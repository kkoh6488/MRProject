using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Handles displaying received query data in the appropriate tabs and menus.
/// </summary>
public class ContentManager : AppMonoBehaviour {

    private enum ActiveModule
    {
        GOOGLE = 0,
        RELATED = 1,
        HISTORY = 2,
    }

    public RectTransform bodyRT;
    public Vector2 bodyDisplayPos;
    public GoogleGraphPanel googleGraphPanel;
    public RelatedPanel relatedPanel;
    public HistoryPanel historyPanel;
    public Text emptyMessage;

    // State variables
    public bool isPanelShown { get; private set; }
    private ActiveModule _currModule = ActiveModule.GOOGLE;
    private ActiveModule _lastModule;

    // Animation variables
    private Vector2 _bodyRTOffscreen;
    private bool _isSlidingIn = false;
    private bool _isSlidingOut = false;
    private float _minDelta = 0.05f;
    private Vector2 _currVel;
    private float _smoothTime = 0.3f;

    // Content size variables
    public RectTransform bodyScrollContent;
    private Vector2 _defaultContentSize;


    private QueryResult[] _lastResults;
    private bool _newResults = false;

    // Events
    public UnityEvent OnPanelShow;
    public UnityEvent OnPanelHide;

    // Use this for initialization
    void Start () {
        _bodyRTOffscreen = bodyRT.anchoredPosition;
        bodyDisplayPos = _bodyRTOffscreen + new Vector2(150f, 0);
        _defaultContentSize = bodyScrollContent.sizeDelta;
	}
	
	// Update is called once per frame
	void Update () {
        if (_lastResults == null)
        {
            emptyMessage.gameObject.SetActive(true);
        }
        else
        {
            emptyMessage.gameObject.SetActive(false);
        }
        if (_newResults)
        {
            _currModule = ActiveModule.GOOGLE;
            _newResults = false;
            _isSlidingOut = false;
            DisplayMainResult(_lastResults[0]);
            SlideInResults();
            HandleRelatedResults(_lastResults);
        }
	    if (_isSlidingIn)
        {
            SlideInResults();
        }
        else if (_isSlidingOut)
        {
            SlideOutResults();
        }
        DisplayActiveModulePanel();
	}

    public void HandleResults(QueryResult[] results)
    {
        _lastResults = results;
        _newResults = true;
    }

    
    public void ShowRelatedResult(int id)
    {
        _currModule = ActiveModule.GOOGLE;
        DisplayActiveModulePanel();
        googleGraphPanel.SetDisplayContent(_lastResults[id]);
        bodyScrollContent.anchoredPosition = Vector2.zero;
    }

    private void DisplayMainResult(QueryResult q)
    {
        _isSlidingIn = true;
        bodyRT.anchoredPosition = _bodyRTOffscreen;
        googleGraphPanel.SetDisplayContent(q);
    }

    private void HandleRelatedResults(QueryResult[] results)
    {
        relatedPanel.HandleRelatedResults(results);
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

    public void SetActiveContentPanel(int module)
    {
        if (module == 0)
        {
            _currModule = ActiveModule.GOOGLE;
        }
        else if (module == 1)
        {
            _currModule = ActiveModule.RELATED;
        }
        else
        {
            _currModule = ActiveModule.HISTORY;
        }
        bodyScrollContent.anchoredPosition = Vector2.zero;
    }

    private void DisplayActiveModulePanel()
    {
        if (_currModule == ActiveModule.GOOGLE)
        {
            if (_lastResults == null)
            {
                googleGraphPanel.gameObject.SetActive(false);
            }
            else
            {
                googleGraphPanel.gameObject.SetActive(true);
                bodyScrollContent.sizeDelta = new Vector2(_defaultContentSize.x, googleGraphPanel.contentHeight);
            }
            relatedPanel.gameObject.SetActive(false);
            historyPanel.gameObject.SetActive(false);
        }
        else if (_currModule == ActiveModule.RELATED)
        {
            googleGraphPanel.gameObject.SetActive(false);
            relatedPanel.gameObject.SetActive(true);
            historyPanel.gameObject.SetActive(false);
            bodyScrollContent.sizeDelta = new Vector2(_defaultContentSize.x, relatedPanel.contentHeight);
        }
        else
        {
            googleGraphPanel.gameObject.SetActive(false);
            relatedPanel.gameObject.SetActive(false);
            historyPanel.gameObject.SetActive(true);
            //bodyScrollContent.sizeDelta = new Vector2(_defaultContentSize.x, relatedPanel.contentHeight);
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
