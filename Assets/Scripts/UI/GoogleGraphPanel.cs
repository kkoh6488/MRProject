using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// Handles displaying results in the sidebar panel for Google graph search.
/// </summary>
public class GoogleGraphPanel : ContentPanel {

    private struct CachedImage
    {
        public CachedImage(string title, byte[] bytes, Vector2 size)
        {
            this.title = title;
            this.bytes = bytes;
            this.size = size;
        }

        public string title;
        public byte[] bytes;
        public Vector2 size;
    }

    public Renderer imgQuad;
    public Text title;
    public Text subtitle;
    public Text description;
    public Image[] subimages;
    public GameObject fieldPrefab;
    public Transform fieldParent;
    public Image img;

    private float _imgHeight;
    private float _imgWidth;

    private float _fieldHeightPx = 30f;
    private Material _imgMat;
    //private string _cachePath;
    Queue<CachedImage> _cacheQueue;

    // Use this for initialization
    void Start () {
        _imgMat = imgQuad.material;
        _imgWidth = img.rectTransform.sizeDelta.x;
        _imgHeight = img.rectTransform.sizeDelta.y;
        _cacheQueue = new Queue<CachedImage>();
        //_cachePath = Application.persistentDataPath + "/";
	}
	
	// Update is called once per frame
	void Update () {
        if (_cacheQueue.Count > 0)
        {
            CacheImage(_cacheQueue.Dequeue());
        }
	}

    /// <summary>
    /// Displays the given query result in the Google Graph content panel.
    /// </summary>
    /// <param name="q">The query result to be displayed.</param>
    public override void SetDisplayContent(QueryResult q)
    {
        if (!IsImageCached(q.title))
        {
            StartCoroutine(LoadImageFromURL(q.imgUrl, q.title));
        }
        else
        {
            LoadImageFromCache(q.title);
        }
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
        Debug.Log("Main Result:" + q);
    }
    
    IEnumerator LoadImageFromURL(string url, string title)
    {
        WWW www = new WWW(url);
        yield return www;
        imgQuad.material.mainTexture = www.texture;
        //www.LoadImageIntoTexture((Texture2D) _imgMat.mainTexture);
        byte[] bytes = www.texture.EncodeToJPG();
        _cacheQueue.Enqueue(new CachedImage(title, bytes, new Vector2(www.texture.height, www.texture.width)));
        LoadImageIntoSprite(bytes, www.texture.width, www.texture.height);
    }

    void CacheImage(CachedImage c)
    {
        if (File.Exists(Application.persistentDataPath + c.title))
        {
            File.Delete(Application.persistentDataPath + c.title);
        }
        Debug.Log("Writing");
        Debug.Log(Application.persistentDataPath + c.title);
        string filename = c.size.y + "x" + c.size.x + "_" + c.title + ".jpg";
        File.WriteAllBytes(Application.persistentDataPath + filename, c.bytes);
    }

    void LoadImageFromCache(string title)
    {
        string filepath = Directory.GetFiles(Application.persistentDataPath, "*" + title + ".jpg")[0];
        Debug.Log("Reading from cache:" + filepath);
        byte[] bytes = File.ReadAllBytes(filepath);
        FileInfo f = new FileInfo(filepath);
        //Store size in filename widthxheight_id
        int sizeDelimiter = f.Name.IndexOf('x');
        int underscore = f.Name.IndexOf('_');
        int width = int.Parse(f.Name.Substring(0, sizeDelimiter));
        int height = int.Parse(f.Name.Substring(sizeDelimiter + 1, underscore - sizeDelimiter - 1));
        Debug.Log(width + " " + height);
        
        LoadImageIntoSprite(bytes, width, height);
    }

    void LoadImageIntoSprite(byte[] bytes, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.LoadImage(bytes);
        Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(width, height)), Vector2.zero);
        img.sprite = sprite;
    }

    bool IsImageCached(string title)
    {
        Debug.Log("Title: " + title + " , " + Application.persistentDataPath);
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*" + title + ".jpg");
        if (files != null)
        {
            return (files.Length > 0);
        }
        return false;
    }
}
