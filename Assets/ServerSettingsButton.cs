using UnityEngine;
using UnityEngine.UI;

public class ServerSettingsButton : AppMonoBehaviour {

    public InputField serverIP;
    public InputField port;

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable()
    {
        UpdateServerAddress();
    }
	
	public void OnClick()
    {
        manager.SetServerAddress(serverIP.text, int.Parse(port.text));
    }

    private void UpdateServerAddress()
    {
        serverIP.text = manager.serverIp;
        port.text = manager.port.ToString();
    }
}
