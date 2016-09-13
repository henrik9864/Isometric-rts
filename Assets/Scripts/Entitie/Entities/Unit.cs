using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
public class Unit : Entitie, IDamageable, IHighlightable, ISelectable
{
    UnityEngine.AI.NavMeshAgent agent;
    Team unitTeam;
    public Team team
    {
        get
        {
            return unitTeam;
        }
    }

    bool highlighted;
    Texture2D highlightedTexture;

    public override Texture2D image
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

    [Space (10)] // Make space for the unit section.

    [SerializeField]
    protected float unitProvokekRange = 1f;
    public float provokeRange
    {
        get
        {
            return unitProvokekRange;
        }
    }

    [SerializeField]
    protected float unitAttackRange = 1f;
    public float attackRange
    {
        get
        {
            return unitAttackRange;
        }
    }

    [SerializeField]
    public float unitHealth; // Change back to protected
    public float health
    {
        get
        {
            return unitHealth;
        }
    }

    [SerializeField]
    protected float unitDamage;
    public float damage
    {
        get
        {
            return unitDamage;
        }
    }

    public Rect hitbox
    {
        get
        {
            return EntitieRenderer.getEntitieHitbox (this);
        }
    }

    public event InterfaceEventHandler _onKilled;

    protected override void Start ()
    {
        base.Start ();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        highlightedTexture = TextureBaker.generateHighlight (entImage, 1, Color.black);
    }

    public void Move ( Vector3 pos )
    {
        if (agent != null)
        {
            agent.SetDestination (pos);
        }
    }

    public IEnumerator Flock ( Vector3 pos )
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath ();
        UnityEngine.AI.NavMesh.CalculatePath (transform.position, pos, UnityEngine.AI.NavMesh.AllAreas, path);
        agent.SetPath (path);
        anim.ChangeAnimation ("RunN");

        yield return new WaitUntil (() => { return agent.remainingDistance < 1; });
        yield return new WaitForSeconds (agent.remainingDistance / agent.speed);

        agent.ResetPath ();
        anim.ChangeAnimation ("IdleN");
    }

    public IEnumerator Attack ( Unit target )
    {
        while (target != null)
        {
            if (this != null)
            {
                Move (target.transform.position);

                if (Vector3.Distance (transform.position, target.transform.position) <= attackRange)
                {
                    target.TakeDamage (unitDamage);
                }

                yield return new WaitForSeconds (.2f);
            }
            else
            {
                break;
            }
        }

    }

    public void SetTeam ( Team team )
    {
        unitTeam = team;
        team.RegisterUnit (this);
    }

    public void TakeDamage ( float damage )
    {
        unitHealth -= damage;

        if (unitHealth <= 0)
        {
            Kill ();
        }
    }

    public void ToggleHighlight ( bool state )
    {
        highlighted = state;
    }

    void Kill ()
    {
        StopAllCoroutines (); // Stop all coroutines that may refference this unit.
        team.UnregisterUnit (this); // remove this unit from its team.
        if (_onKilled != null)
        {
            _onKilled (this);
        }

        Destroy (gameObject);
    }
}
