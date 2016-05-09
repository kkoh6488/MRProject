using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GoogleGraphPanel : ContentPanel {

    public Image img;
    public Text title;
    public Text subtitle;
    public Text description;
    public Image[] subimages;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void SetDisplayContent(QueryResult q)
    {
        title.text = q.title;
        subtitle.text = q.subtitle;
        description.text = q.description;
        foreach (string s in q.fields.Keys)
        {

        }
    }

}
