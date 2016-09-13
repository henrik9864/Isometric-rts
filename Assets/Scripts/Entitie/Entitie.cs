using UnityEngine;
using System.Collections;

public class Entitie : MonoBehaviour
{
    [SerializeField]
    protected Texture2D entImage;
    [SerializeField]
    protected float entScale = 1;

    protected EntitieAnimator anim;
    protected bool hasAnim;

    public virtual Texture2D image
    {
        get
        {
            if (hasAnim)
            {
                return (Texture2D)anim.getCurrentFrame (false);
            }
            else
            {
                return entImage;
            }
        }

        protected set
        {
            entImage = value;
        }
    }

    public float scale
    {
        get
        {
            return entScale;
        }
    }

    protected virtual void Start ()
    {
        anim = gameObject.GetComponent<EntitieAnimator> ();
        if (anim != null)
        {
            hasAnim = true;
            anim.BakeFrames ();
        }
    }
}
