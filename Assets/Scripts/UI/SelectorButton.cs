using UnityEngine;
using UnityEngine.Events;

public class SelectorButton : MonoBehaviour {

    public SelectorGroup group;

    public UnityEvent OnSelect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnClick()
    {
        group.Select(this);
        OnSelect.Invoke();
    }
}
