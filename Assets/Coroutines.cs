using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    public static Coroutines instance;

    void Awake ()
    {
        instance = this;
    }
}

