using UnityEngine;
using UnityEngine.UI;

public class ToggleableIcon : MonoBehaviour {

    public Sprite offSprite;
    public Sprite onSprite;

    private Image _img;
    private bool _isOn = false;

    void Start()
    {
        _img = GetComponent<Image>();
    }

    public void Toggle()
    {
        if (_isOn)
        {
            _img.sprite = offSprite;
        }
        else
        {
            _img.sprite = onSprite;
        }
        _isOn = _isOn ? false : true;
    }
}
