using System.Collections.Generic;

/// <summary>
/// Contains data from a successful query result.
/// </summary>
public class QueryResult {

    public string title { get; private set; }
    public string subtitle { get; private set; }
    public string img { get; private set; }
    public string description { get; private set; }
    public Dictionary<string, string> fields { get; private set; }

    public QueryResult(string title, string subtitle, string description, string img)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.description = description;
        this.img = img;
        fields = new Dictionary<string, string>();
    }

    void SetField(string key, string value)
    {
        fields.Add(key, value);
    }

    public bool HasDescription()
    {
        return description != null;
    }

    public bool HasImage()
    {
        return img != null;
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
