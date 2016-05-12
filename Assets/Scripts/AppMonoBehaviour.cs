using UnityEngine;

public class AppMonoBehaviour : MonoBehaviour {

    protected IRManager manager;
    protected AppState currentState
    {
        get { return manager.currentState; }
        set { manager.currentState = value; }
    }

    void Awake()
    {
        manager = GameObject.FindObjectOfType<IRManager>();
    }

    public void SetState(AppState state)
    {
        currentState = state;
    }
}
