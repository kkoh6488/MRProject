using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RotatingTouchCube : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler {

    Vector3 randomDir;
    public float speedMultipler;

    private Rigidbody _rb;
    private bool _isHeld;
    private Vector3 _lastDownPos;
    private Vector3 _force = Vector3.one;
    private float _minVal = 0.3f;
    private float _maxVal = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!_isHeld)
        {
            transform.Rotate(randomDir * speedMultipler);
        }
	}

    void OnEnable()
    {
        randomDir = new Vector3(Random.RandomRange(_minVal, _maxVal), Random.RandomRange(_minVal, _maxVal), Random.RandomRange(_minVal, _maxVal));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Adding force");
        _rb.AddForceAtPosition(_force, _lastDownPos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
        _lastDownPos = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
    }
}
