using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InterfaceEventHandler ( Unit dead );
public interface IDamageable
{
    event InterfaceEventHandler _onKilled;

    void TakeDamage ( float amount );
}