using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group
{
    public Queue<Node> members = new Queue<Node> ();
    public int id;

    public Group(int _id )
    {
        id = _id;
    }
}
