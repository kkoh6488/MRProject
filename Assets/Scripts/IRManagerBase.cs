using UnityEngine;
using System;

public abstract class IRManagerBase : MonoBehaviour, IManager
{
    public abstract bool CaptureImage();

    public abstract void SaveImage(string filename);
}
