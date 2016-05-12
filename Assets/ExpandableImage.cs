using UnityEngine;

public class ExpandableImage : MonoBehaviour {

    private enum ExpandState
    {
        UNEXPANDED = 0,
        EXPANDED = 1,
    }

    public Transform expandAnchor;
    public float scaleFactor;
    private bool _isExpanded;
    private bool _isAnimating;
    private ExpandState _targetState;

    void Update()
    {
        if (_targetState == ExpandState.UNEXPANDED && _isExpanded)
        {

        }
        else if (_targetState == ExpandState.EXPANDED && !_isExpanded)
        {

        }
    }

    public void OnClick()
    {
        // Invert the flags for expansion
        _targetState = _isExpanded ? ExpandState.UNEXPANDED : ExpandState.EXPANDED;
        _isExpanded = _isExpanded ? false : true;
    }
}
