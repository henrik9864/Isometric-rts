using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    void Start ()
    {

    }

    public void Move ( Vector3 dir, float speed )
    {
        transform.Translate (dir * speed * Time.deltaTime);
    }
}

[System.Serializable]
public class Controls
{
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public int rightClick = 0;
    public int leftClick = 1;
}