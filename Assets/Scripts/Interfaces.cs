using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InterfaceEventHandler ( Unit dead );
public interface IDamageable
{
    event InterfaceEventHandler _onKilled;

    void TakeDamage ( float amount );
}

public interface IHighlightable
{
    void ToggleHighlight ( bool state );

    Texture2D image
    {
        get;
    }
}

public interface ISelectable
{
    Rect hitbox
    {
        get;
    }
}