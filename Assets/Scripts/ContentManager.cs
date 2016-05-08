using UnityEngine;
using System.Collections;

/// <summary>
/// Handles displaying received query data in the appropriate tabs and menus.
/// </summary>
public class ContentManager : AppMonoBehaviour {

    public RectTransform bodyRT;
    public RectTransform topBarRT;
    public Vector2 bodyDisplayPos = Vector2.zero;

    // Animation variables
    private Vector2 _bodyRTOffscreen;
    private bool _isSlidingIn = false;
    private float _minDelta = 0.05f;
    private Vector2 _currVel;
    private float _smoothTime = 0.3f;

	// Use this for initialization
	void Start () {
        _bodyRTOffscreen = bodyRT.anchoredPosition;
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (_isSlidingIn)
        {
            SlideInResults();
        }
	}

    public void HandleResult()
    {
        _isSlidingIn = true;
        bodyRT.anchoredPosition = _bodyRTOffscreen;
        topBarRT.anchoredPosition = _bodyRTOffscreen;
    }

    private void SlideInResults()
    {
        if (Vector2.Distance(bodyRT.anchoredPosition, bodyDisplayPos) < _minDelta)
        {
            _isSlidingIn = false;
            return;
        }
        else
        {
            //bodyRT.anchoredPosition = Vector2.MoveTowards(bodyRT.anchoredPosition, bodyDisplayPos.anchoredPosition, moveSpeed);
            bodyRT.anchoredPosition = Vector2.SmoothDamp(bodyRT.anchoredPosition, bodyDisplayPos, ref _currVel, _smoothTime);
            topBarRT.anchoredPosition = Vector2.SmoothDamp(bodyRT.anchoredPosition, bodyDisplayPos, ref _currVel, _smoothTime);
        }
    }
}
