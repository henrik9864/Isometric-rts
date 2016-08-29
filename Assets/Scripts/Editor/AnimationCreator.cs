using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AnimationCreator : EditorWindow
{
    Animation[] aniamtionArray = new Animation[0];
    List<Hitbox> hitboxes = new List<Hitbox> ();

    string frameWidth = "32";
    string frameHeight = "32";
    string fps = "10";
    string animations = "1";

    int lastHoverAnim = -1;

    Texture2D bacgroundTile;

    [MenuItem ("Window/Animation Creator")]
    public static void ShowWindow ()
    {
        GetWindow (typeof (AnimationCreator));
    }

    void OnEnable ()
    {
        OnFocuss ();
    }

    void OnFocuss ()
    {
        if (Selection.activeGameObject != null)
        {
            EntitieAnimator animator = Selection.activeGameObject.GetComponent<EntitieAnimator> ();

            if (animator != null)
            {
                EntitieAnimation[] entAnimations = animator.animations;
                animations = entAnimations.Length.ToString ();
                System.Array.Resize (ref aniamtionArray, entAnimations.Length);

                for (int i = 0; i < entAnimations.Length; i++)
                {
                    aniamtionArray[i] = new Animation ();
                    aniamtionArray[i].name = entAnimations[i].name;
                    foreach (Frame frame in entAnimations[i].sequence)
                    {
                        aniamtionArray[i].frames.Add (frame.texture);
                        hitboxes.Add (new Hitbox (int.Parse (frameWidth), int.Parse (frameHeight), i, aniamtionArray[i].frames.Count - 1));
                    }
                }
            }

            Repaint ();
        }
    }

    void OnSelectionChange ()
    {
        //OnFocus ();
    }

    void OnGUI ()
    {
        GUI.Box (new Rect (0, 0, position.width, 50), "");
        GUI.Box (new Rect (0, 49, 100, position.height), "");

        GUI.Label (new Rect (10, 10, 40, 15), "Width: ");
        frameWidth = GUI.TextField (new Rect (50, 10, 55, 15), frameWidth, 4);
        frameWidth = Regex.Replace (frameWidth, @"[^Z0-9 ]", "");

        GUI.Label (new Rect (10, 30, 40, 15), "Height: ");
        frameHeight = GUI.TextField (new Rect (50, 30, 55, 15), frameHeight, 4);
        frameHeight = Regex.Replace (frameHeight, @"[^Z0-9 ]", "");

        GUI.Label (new Rect (120, 10, 40, 15), "Fps: ");
        fps = GUI.TextField (new Rect (160, 10, 55, 15), fps, 4);
        fps = Regex.Replace (fps, @"[^Z0-9 ]", "");

        GUI.Label (new Rect (120, 30, 70, 15), "Animations: ");
        animations = GUI.TextField (new Rect (190, 30, 55, 15), animations, 4);
        animations = Regex.Replace (animations, @"[^Z0-9 ]", "");

        if (GUI.Button (new Rect (260, 5, 70, 15), "Reset"))
        {
            aniamtionArray = new Animation[0];
            hitboxes.RemoveRange (0, hitboxes.Count);
        }

        if (GUI.Button (new Rect (260, 25, 70, 15), "Export") && Selection.activeGameObject != null)
        {
            EntitieAnimator animator = Selection.activeGameObject.GetComponent<EntitieAnimator> ();

            if (Selection.activeGameObject.GetComponent<Entitie> () == null)
            {
                Selection.activeGameObject.AddComponent<Entitie> ();
            }
            if (animator == null)
            {
                animator = Selection.activeGameObject.AddComponent<EntitieAnimator> ();
            }

            animator.ResetAnimations ();

            foreach (Animation anim in aniamtionArray)
            {
                EntitieAnimation animation = new EntitieAnimation (anim.name, int.Parse (fps));

                foreach (Texture tex in anim.frames)
                {
                    animation.AddFrame (tex);
                }

                animator.AddAnimation (animation);
            }
        }

        if (GUI.Button (new Rect (340, 5, 70, 15), "Preview"))
        {
            GetWindow<AnimationPreview> ().LoadAnimations (new List<Animation> (aniamtionArray), int.Parse (fps));
        }

        if (GUI.Button (new Rect (340, 25, 70, 15), "Import"))
        {
            OnFocuss ();
        }

        if (animations != "" && frameHeight != "" && frameWidth != "")
        {
            int fHeight = int.Parse (frameHeight);
            int fWidth = int.Parse (frameWidth);
            int animAmount = int.Parse (animations);

            if (aniamtionArray.Length != animAmount)
            {
                System.Array.Resize (ref aniamtionArray, animAmount);

                for (int i = 0; i < aniamtionArray.Length; i++)
                {
                    Animation anim = aniamtionArray[i];
                    if (anim == null)
                    {
                        aniamtionArray[i] = new Animation ();
                    }
                }
            }

            for (int i = 0; i < animAmount; i++)
            {
                Color defColor = GUI.color;
                GUI.color = aniamtionArray[i].hover ? Color.grey : new Color (.6f, .6f, .6f, 1);
                GUI.Box (new Rect (99, 49 + ((fHeight - 1) * i), position.width, fHeight), "");

                GUI.color = defColor;
                aniamtionArray[i].name = GUI.TextField (new Rect (10, 50 + fHeight * i + (fHeight / 2 - 7.5f), 80, 15), aniamtionArray[i].name);
            }

            for (int y = 0; y < aniamtionArray.Length; y++)
            {
                Animation anim = aniamtionArray[y];
                for (int x = 0; x < anim.frames.Count; x++)
                {
                    GUI.DrawTexture (new Rect (100 + (fWidth * x), 49 + ((fHeight - 1) * y), fWidth, fHeight), anim.frames[x]);
                }
            }

            switch (Event.current.type)
            {
                case EventType.MouseUp:
                    Vector2 mousePos = Event.current.mousePosition;

                    for (int i = 0; i < hitboxes.Count; i++)
                    {
                        if (hitboxes[i].hitbox.Contains (mousePos))
                        {
                            Hitbox hitbox = hitboxes[i];
                            aniamtionArray[hitbox.animation].frames.RemoveAt (hitbox.animationIndex);
                            hitboxes.RemoveAt (i);
                            Repaint ();
                            break;
                        }
                    }
                    break;
                case EventType.DragUpdated:
                    mousePos = Event.current.mousePosition;

                    int animHover = Mathf.FloorToInt ((mousePos.y - 50) / fHeight);
                    if (animHover >= 0 && animHover < aniamtionArray.Length)
                    {
                        aniamtionArray[animHover].hover = true;

                        if (animHover != lastHoverAnim && lastHoverAnim >= 0)
                        {
                            aniamtionArray[lastHoverAnim].hover = false;
                        }

                        lastHoverAnim = animHover;
                        Repaint ();
                    }
                    break;
                case EventType.DragExited:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.AcceptDrag ();

                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (obj.GetType () == typeof (Texture2D) || obj.GetType () == typeof (Texture))
                        {
                            Texture tex = (Texture)obj;
                            aniamtionArray[lastHoverAnim].frames.Add (tex);
                            aniamtionArray[lastHoverAnim].hover = false;
                            hitboxes.Add (new Hitbox (fHeight, fWidth, lastHoverAnim, aniamtionArray[lastHoverAnim].frames.Count - 1));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class Animation
    {
        public string name = "Animation";
        public bool hover = false;
        public List<Texture> frames = new List<Texture> ();
    }

    class Hitbox
    {
        public Rect hitbox;
        public int animation;
        public int animationIndex;

        public Hitbox ( int fHeight, int fWidth, int animation, int animationIndex )
        {
            this.hitbox = new Rect (100 + (fWidth * animationIndex), 49 + ((fHeight - 1) * animation), fWidth, fHeight);
            this.animation = animation;
            this.animationIndex = animationIndex;
        }
    }
}