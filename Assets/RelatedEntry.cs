using UnityEngine.UI;

public class RelatedEntry : AppMonoBehaviour {

    public Text titleLabel;
    public Text subtitleLabel;
    public int id;

    public void InitialiseEntry(string title, string subtitle, int id)
    {
        titleLabel.text = title;
        subtitleLabel.text = subtitle;
        this.id = id;
    }

    public void ShowEntryInMainWindow()
    {
        manager.content.ShowRelatedResult(id);
    }
}
