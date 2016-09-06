using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class EntitieAnimator : MonoBehaviour
{
    [SerializeField]
    List<EntitieAnimation> entAnimations = new List<EntitieAnimation> ();
    public EntitieAnimation[] animations
    {
        get
        {
            return entAnimations.ToArray ();
        }
    }

    [SerializeField]
    int selectedAnim = 0;
    int animState = 0;
    float timeToNextFrame = 0;

    void Update ()
    {
        if (timeToNextFrame <= Time.time)
        {
            EntitieAnimation anim = animations[selectedAnim];

            timeToNextFrame = Time.time + 1 / anim.fps;

            animState++;
            if (animState >= anim.sequence.Length)
            {
                animState = 0;
            }
        }
    }

    public Texture getCurrentFrame ( bool highlighted )
    {
        if (highlighted)
        {
            return animations[selectedAnim].sequence[animState].highlightTexture;
        }
        else
        {
            return animations[selectedAnim].sequence[animState].texture;
        }
    }

    public void ChangeAnimation ( int animID )
    {
        selectedAnim = animID;
        animState = 0;
    }

    public void ChangeAnimation ( string animName )
    {
        int index = entAnimations.FindIndex (( a ) => { return a.name == animName; });
        if (index >= 0)
        {
            selectedAnim = index;
            animState = 0;
        }
        else
        {
            Debug.LogWarning ("Aniamton name \"" + @animName + "\" was not found on entitie \"" + @name + "\"", gameObject);
        }
    }

    public void AddAnimation ( EntitieAnimation animation )
    {
        entAnimations.Add (animation);
    }

    public void ResetAnimations ()
    {
        entAnimations = new List<EntitieAnimation> ();
    }

    public void BakeFrames ()
    {
        foreach (EntitieAnimation anim in entAnimations)
        {
            foreach (Frame frame in anim.sequence)
            {
                if (frame.highlightTexture == null)
                {
                    frame.highlightTexture = TextureBaker.generateHighlight ((Texture2D)frame.texture, 1, Color.black);
                }
            }
        }
    }
}

[System.Serializable]
public class EntitieAnimation
{
    public string name;
    public float fps;

    [SerializeField]
    Frame[] animSequence = new Frame[0];
    public Frame[] sequence
    {
        get
        {
            return animSequence;
        }
    }

    public EntitieAnimation ( string name, float fps )
    {
        this.name = name;
        this.fps = fps;
    }

    public void AddFrame ( Texture frame )
    {
        System.Array.Resize (ref animSequence, animSequence.Length + 1);
        animSequence[animSequence.Length - 1] = new Frame (frame);
    }

    public void AddFrames ( List<Texture> frames )
    {
        int startIndex = animSequence.Length;
        System.Array.Resize (ref animSequence, animSequence.Length + frames.Count);

        for (int i = 0; i < frames.Count; i++)
        {
            animSequence[startIndex + i] = new Frame (frames[i]);
        }
    }
}

[System.Serializable]
public class Frame
{
    [HideInInspector]
    public string name;
    public Texture texture;
    public Texture highlightTexture;

    public Frame ( Texture texture )
    {
        this.texture = texture;
        name = texture.name;
        highlightTexture = TextureBaker.generateHighlight ((Texture2D)texture, 1, Color.black);
    }
}