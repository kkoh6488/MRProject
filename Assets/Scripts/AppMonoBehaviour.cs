using UnityEngine;

public class AppMonoBehaviour : MonoBehaviour {

    protected IRManager manager;

    void Awake()
    {
        manager = GameObject.FindObjectOfType<IRManager>();
    }
}
