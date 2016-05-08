using UnityEngine;

public class AppMonoBehaviour : MonoBehaviour {

    private IRManagerBase manager;

    void Awake()
    {
        manager = GameObject.FindObjectOfType<IRManagerBase>();
    }
}
