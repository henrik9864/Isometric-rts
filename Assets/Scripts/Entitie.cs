using UnityEngine;
using System.Collections;

public class Entitie : MonoBehaviour
{
    [SerializeField]
    Texture2D entImage;
    [SerializeField]
    float entScale = 1;

    bool highlighted;
    Texture2D highlightedTexture;

    protected EntitieAnimator anim;
    protected bool hasAnim;

    public Texture2D image
    {
        get
        {
            if (hasAnim)
            {
                return (Texture2D)anim.getCurrentFrame (highlighted);
            }
            else if (highlighted)
            {
                return highlightedTexture;
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

    public void ToggleHighlight ( bool state )
    {
        highlighted = state;
    }

    protected virtual void Start ()
    {
        highlightedTexture = TextureBaker.generateHighlight (entImage, 1, Color.black);
        anim = gameObject.GetComponent<EntitieAnimator> ();
        if (anim != null)
        {
            hasAnim = true;
        }
    }
}
