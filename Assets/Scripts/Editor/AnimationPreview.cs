using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AnimationPreview : EditorWindow
{
    List<AnimationCreator.Animation> animations = new List<AnimationCreator.Animation> ();

    AnimationCreator.Animation selectedAnimation;
    float timeToNextFrame = 0;
    int animState = 0;
    int fps = 0;

    public static void ShowWindow ()
    {
        GetWindow (typeof (AnimationPreview));
    }

    void OnLostFocus ()
    {
        Close ();
    }

    void Update ()
    {

        if (timeToNextFrame <= Time.realtimeSinceStartup)
        {
            Debug.Log (fps);
            timeToNextFrame = Time.realtimeSinceStartup + (1f / fps);

            animState++;
            if (animState >= selectedAnimation.frames.Count)
            {
                animState = 0;
            }
        }

        if (selectedAnimation != null)
        {
            Repaint ();
        }
    }

    void OnGUI ()
    {
        GUI.Box (new Rect (0, 0, 100, Screen.height), "");

        for (int i = 0; i < animations.Count; i++)
        {
            if (GUI.Button (new Rect (10, 10 + 25 * i, 80, 15), animations[i].name))
            {
                selectedAnimation = animations[i];
            }
        }

        if (selectedAnimation != null && selectedAnimation.frames.Count > 0 && fps != 0)
        {
            GUI.DrawTexture (new Rect (110, 10, 128, 128), selectedAnimation.frames[animState]);
        }
    }

    public void LoadAnimations ( List<AnimationCreator.Animation> animations, int fps )
    {
        this.fps = fps;
        this.animations = animations;
        selectedAnimation = animations[0];
    }
}
