using UnityEngine;
using System.Collections;

public class SelectorButton : MonoBehaviour {

    public SelectorGroup group;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnClick()
    {
        group.Select(this);
    }
}
