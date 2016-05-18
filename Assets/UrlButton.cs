using UnityEngine;

public class UrlButton : MonoBehaviour {

    public string url;
    
    public void LaunchWebpage()
    {
        Application.OpenURL(url);
    }
}
