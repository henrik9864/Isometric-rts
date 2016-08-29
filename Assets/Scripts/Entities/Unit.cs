using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Unit : Entitie
{
    NavMeshAgent agent;

    protected override void Start ()
    {
        base.Start ();
        agent = GetComponent<NavMeshAgent> ();
    }

    public void Move ( Vector3 pos )
    {
        agent.SetDestination (pos);
    }

    public IEnumerator Flock ( Vector3 pos )
    {
        NavMeshPath path = new NavMeshPath ();
        NavMesh.CalculatePath (transform.position, pos, NavMesh.AllAreas, path);
        agent.SetPath (path);
        anim.ChangeAnimation ("RunN");

        yield return new WaitUntil (() => { return agent.remainingDistance < 1; });
        yield return new WaitForSeconds (agent.remainingDistance / agent.speed);

        agent.ResetPath ();
        anim.ChangeAnimation ("Idlek");
    }
}
