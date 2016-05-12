﻿using UnityEngine;
using System.Collections;

public class SelectorGroup : MonoBehaviour {

    // Animation variables
    public RectTransform selector;
    public float moveSpeed = 0.0025f;

    private bool _isSelectorMoving = false;
    private float _minDelta = 0.005f;

    private SelectorButton _selected;
    private RectTransform _selectedRT;
    private Vector2 _currVel;
    private float _smoothTime = 0.3f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isSelectorMoving)
        {
            MoveSelector();
        }
    }

    public void Select(SelectorButton button)
    {
        _selected = button;
        _isSelectorMoving = true;
        _selectedRT = button.GetComponent<RectTransform>();
    }

    void MoveSelector()
    {
        if (Vector2.Distance(selector.anchoredPosition, _selectedRT.anchoredPosition) < _minDelta)
        {
            _isSelectorMoving = false;
            return;
        }
        else
        {
            //selector.anchoredPosition = Vector2.MoveTowards(selector.anchoredPosition, _selectedRT.anchoredPosition, moveSpeed);
            selector.anchoredPosition = Vector2.SmoothDamp(selector.anchoredPosition, _selectedRT.anchoredPosition, ref _currVel, _smoothTime);
        }
    }
}