using System.Collections.Generic;

/// <summary>
/// Contains data from a successful query result.
/// </summary>
public class QueryResult {

    public string title { get; private set; }
    public string subtitle { get; private set; }
    public string imgUrl { get; private set; }
    public string wikiUrl { get; private set; }
    public string description { get; private set; }
    public float score { get; private set; }
    public Dictionary<string, string> fields { get; private set; }

    public QueryResult(string title, string subtitle, string description, string imgUrl, string wikiUrl, float score)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.description = description;
        this.imgUrl = imgUrl;
        this.wikiUrl = wikiUrl;
        this.score = score;
        fields = new Dictionary<string, string>();
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}, {4}: {5}", title, subtitle, description, imgUrl, wikiUrl, score);
    }

    public void SetField(string key, string value)
    {
        fields.Add(key, value);
    }

    public bool HasDescription()
    {
        return description != null;
    }

    public bool HasImage()
    {
        return imgUrl != null;
    }
    
    public bool HasTitle()
    {
        return title != null;
    }

    public bool HasSubtitle()
    {
        return subtitle != null;
    }
}
