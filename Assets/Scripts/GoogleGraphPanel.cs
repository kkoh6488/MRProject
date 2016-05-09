using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Handles displaying results in the sidebar panel for Google graph search.
/// </summary>
public class GoogleGraphPanel : ContentPanel {

    public Image img;
    public Text title;
    public Text subtitle;
    public Text description;
    public Image[] subimages;
    public GameObject fieldPrefab;
    public Transform fieldParent;

    private float _fieldHeightPx = 30f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Displays the given query result in the Google Graph content panel.
    /// </summary>
    /// <param name="q">The query result to be displayed.</param>
    public override void SetDisplayContent(QueryResult q)
    {
        title.text = q.title;
        subtitle.text = q.subtitle;
        description.text = q.description;
        Vector2 fieldStartPos = Vector2.zero;
        int fieldCounter = 0;
        foreach (string s in q.fields.Keys)
        {
            GameObject newField = GameObject.Instantiate(fieldPrefab);
            newField.transform.SetParent(fieldParent);
            newField.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            newField.GetComponent<RectTransform>().localScale = Vector3.one;
            newField.GetComponent<RectTransform>().anchoredPosition = fieldStartPos - new Vector2(0, _fieldHeightPx * fieldCounter);
            newField.transform.GetChild(0).GetComponent<Text>().text = s;
            newField.transform.GetChild(1).GetComponent<Text>().text = q.fields[s];
            fieldCounter++;
        }
    }

}
