using UnityEngine;

public class RelatedPanel : MonoBehaviour {

    public GameObject relatedEntryPrefab;
    public Transform resultParent;
    public float contentHeight;

    private int _resultHeight = 50;
    private Vector2 _topRowPos;
    private int _listSize = 10;
    private int _titleHeight = 20;

	// Use this for initialization
	void Start () {
        _topRowPos = relatedEntryPrefab.GetComponent<RectTransform>().anchoredPosition;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HandleRelatedResults(QueryResult[] results)
    {
        ClearResults();
        for (int i = 1; i < _listSize; i++)
        {
            CreateResult(results[i], i);
        }
        contentHeight = _listSize * _resultHeight + _titleHeight;
    }

    private void ClearResults()
    {
        foreach (Transform child in resultParent)
        {
            if (child.name.Contains("Clone"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateResult(QueryResult q, int index)
    {
        GameObject newRow = GameObject.Instantiate(relatedEntryPrefab);
        newRow.SetActive(true);
        newRow.transform.SetParent(resultParent);
        newRow.GetComponent<RectTransform>().position = relatedEntryPrefab.transform.position;
        newRow.transform.localScale = Vector3.one;
        newRow.GetComponent<RectTransform>().anchoredPosition = _topRowPos - new Vector2(0, _resultHeight * (index - 1));
        newRow.GetComponent<RelatedEntry>().InitialiseEntry(q.title, q.subtitle, index);
    }
}
