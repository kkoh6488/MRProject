using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CaptureButton : AppMonoBehaviour {

    public Sprite queryingSprite;
    private Sprite _queryImg;
    private Image _imgComp;
    private bool _isQuerying = false;

    void Awake()
    {
        _imgComp = GetComponent<Image>();
        _queryImg = _imgComp.sprite;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (_isQuerying)
        {
            AnimateQueryIndicator();
        }
	}

    public void OnClick()
    {
        if (!_isQuerying)
        {
            ShowQueryIndicator();
        }
    }

    void ShowQueryIndicator()
    {
        _imgComp.sprite = queryingSprite;
    }

    void AnimateQueryIndicator()
    {

    }
}
