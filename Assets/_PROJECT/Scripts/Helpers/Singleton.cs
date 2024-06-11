using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T GetSharedInstance() => _instance;

    protected static void SetSharedInstance(T value)
    { _instance = value; }

    public void Awake()
    {
        _instance = (T)(object)this;
    }
}
