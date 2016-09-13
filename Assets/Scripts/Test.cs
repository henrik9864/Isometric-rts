using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Texture2D texture1;
    public Texture2D texture2;

    Texture2D toDisplay;

    void Start ()
    {
        toDisplay = TextureBaker.blendTextures (texture1, texture2, texture1, texture2, .5f);
    }

    void OnGUI ()
    {
        GUI.DrawTexture (new Rect (10, 10, toDisplay.width, toDisplay.height), toDisplay);
    }
}
