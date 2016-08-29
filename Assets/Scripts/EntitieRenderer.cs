using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class EntitieRenderer : MonoBehaviour
{

    void OnGUI ()
    {
        DrawEntities ();
    }

    public void DrawEntities ()
    {
        Entitie[] ents = FindObjectsOfType<Entitie> ();
        System.Array.Sort (ents, ( a, b ) => { return b.transform.position.z.CompareTo (a.transform.position.z); });

        foreach (Entitie ent in ents)
        {
            if (ent.image != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint (ent.transform.position);

                float entWidth = ent.image.width * ent.scale;
                float entHeight = ent.image.height * ent.scale;

                GUI.DrawTexture (new Rect (screenPos.x - entWidth / 2, Screen.height - screenPos.y - entHeight, entWidth, entHeight), ent.image);
            }
        }
    }

    public static Rect getEntitieHitbox ( Entitie ent )
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint (ent.transform.position);

        float entWidth = ent.image.width * ent.scale;
        float entHeight = ent.image.height * ent.scale;

        return new Rect (screenPos.x - entWidth / 2, Screen.height - screenPos.y - entHeight, entWidth, entHeight);
    }
}