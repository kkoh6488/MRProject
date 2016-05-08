using UnityEngine;
using System;

public abstract class IRManagerBase : MonoBehaviour, IManager
{
    public AppState currentState;

    public abstract void CaptureImage();

    public abstract void SaveImage(string filename);
}
