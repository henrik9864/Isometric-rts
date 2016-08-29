using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;

    public Vector2 size;
    public int smoothPasses;
    public List<BiomeSetting> biomes = new List<BiomeSetting> ();

    public float scale;
    public float frequency;

    public Tile[,] tileMap;

    void Start ()
    {
        tileMap = new Tile[(int)size.x, (int)size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                GameObject obj = new GameObject ("Tile: " + x + ":" + y);
                obj.transform.parent = transform;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localPosition = new Vector3 (x * 10, 0, y * 10);
                obj.transform.localScale = new Vector3 (10, 1, 10);

                tileMap[x, y] = new Tile (x, y, obj, new BiomeSetting ());

                obj.AddComponent<MeshFilter> ().mesh = mesh;
                MeshRenderer material = obj.AddComponent<MeshRenderer> ();
                material.material.color = Color.grey;
            }
        }

        StartCoroutine (GenerateBiomes ());

        //StartCoroutine (FloodFill (tileMap[2, 2], biomes[Random.Range (0, biomes.Count)], .999f));
    }

    IEnumerator SmoothMap ()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Tile t = tileMap[x, y];

                SmoothTile (t);

                yield return null;
            }

            tileMap[x, 0].obj.GetComponent<MeshRenderer> ().material.color = Color.red;
        }
        //yield return null;
    }

    IEnumerator GenerateBiomes ()
    {

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Tile tile = tileMap[x, y];
                if (!tile.inBiome)
                {
                    yield return StartCoroutine (FloodFill (tile, biomes[Random.Range (0, biomes.Count)], 1.1f));
                }
            }
        }

        //StartCoroutine (SmoothMap ());
    }

    IEnumerator FloodFill ( Tile start, BiomeSetting biomeSettings, float chance )
    {
        Biome biome = new Biome ();
        List<Tile> toAdd = new List<Tile> ();
        toAdd.Add (start);
        start.inBiome = true;

        while (toAdd.Count > 0)
        {
            Tile v = null;
            float distToV = 0;

            for (int i = 0; i < toAdd.Count; i++)
            {
                Tile t = toAdd[i];
                float dist = (new Vector2 (t.x, t.y) - new Vector2 (start.x, start.y)).magnitude;

                if (v == null || distToV > dist)
                {
                    v = t;
                    distToV = dist;
                    toAdd.Remove (t);
                }
            }

            v.obj.GetComponent<MeshRenderer> ().material.color = biomeSettings.color;
            v.inBiome = true;
            v.biome = biome;
            v.settings = biomeSettings;
            biome.tiles.Add (v);

            List<Tile> tiles = getNeighbours (v, toAdd);
            foreach (Tile tile in tiles)
            {
                if (Random.Range (0f, 1f) < chance)
                {
                    toAdd.Add (tile);
                }
            }

            //yield return null;
        }
        yield return null; ;
        print ("Done");

        //return biome;
    }

    public void SmoothTile ( Tile t )
    {
        List<Biome> neighbourBiomes = new List<Biome> ();
        List<int> neighbourBiomeCount = new List<int> ();

        foreach (Tile tile in getNeighbours (t))
        {
            int biomeIndex = neighbourBiomes.FindIndex (( a ) => { return a == tile.biome; });
            Biome biome = neighbourBiomes[biomeIndex];
            if (biome != null)
            {
                neighbourBiomeCount[biomeIndex]++;
            }
            else
            {
                neighbourBiomes.Add (tile.biome);
                neighbourBiomeCount.Add (0);
            }
        }

        print (neighbourBiomes.Count);

        int index = neighbourBiomeCount.FindIndex (( a ) => { return a >= 4; });
        if (index > 0 && index < neighbourBiomes.Count && neighbourBiomes[index] != null)
        {
            t.biome = neighbourBiomes[index];
            t.obj.GetComponent<MeshRenderer> ().material.color = t.settings.color;
        }
    }

    List<Tile> getNeighbours ( Tile tile )
    {
        List<Tile> neighBours = new List<Tile> ();

        for (int x = -1; x < 2; x++)
        {
            if (x != 0)
            {
                if (tile.x + x >= 0 && tile.x + x < size.x)
                {
                    Tile t = tileMap[tile.x + x, tile.y];
                    if (!t.inBiome)
                    {
                        neighBours.Add (t);
                    }
                }
            }
        }

        for (int y = -1; y < 2; y++)
        {
            if (y != 0)
            {
                if (tile.y + y >= 0 && tile.y + y < size.y)
                {
                    Tile t = tileMap[tile.x, tile.y + y];
                    if (!t.inBiome)
                    {
                        neighBours.Add (t);
                    }
                }
            }
        }

        return neighBours;
    }

    List<Tile> getNeighbours ( Tile tile, List<Tile> registerd )
    {
        List<Tile> neighBours = new List<Tile> ();

        for (int x = -1; x < 2; x++)
        {
            if (x != 0)
            {
                if (tile.x + x >= 0 && tile.x + x < size.x)
                {
                    Tile t = tileMap[tile.x + x, tile.y];
                    if (!t.inBiome && !registerd.Contains (t))
                    {
                        neighBours.Add (t);
                    }
                }
            }
        }

        for (int y = -1; y < 2; y++)
        {
            if (y != 0)
            {
                if (tile.y + y >= 0 && tile.y + y < size.y)
                {
                    Tile t = tileMap[tile.x, tile.y + y];
                    if (!t.inBiome && !registerd.Contains (t))
                    {
                        neighBours.Add (t);
                    }
                }
            }
        }

        return neighBours;
    }
}

[System.Serializable]
public class BiomeSetting
{
    public Color color = Color.green;
}

public class Biome
{
    public List<Tile> tiles = new List<Tile> ();
}

public class Tile
{
    public bool inBiome;
    public Biome biome;
    public int x;
    public int y;
    public GameObject obj;
    public BiomeSetting settings;

    public Tile ( int x, int y, GameObject obj, BiomeSetting settings )
    {
        this.x = x;
        this.y = y;
        this.obj = obj;
        this.settings = settings;
    }
}

[CustomEditor (typeof (MapGenerator))]
public class MapgeneratorEditor : Editor
{
    Vector2 size;

    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector ();

        MapGenerator value = (MapGenerator)target;

        size = EditorGUILayout.Vector2Field ("Size", size);

        if (GUILayout.Button ("Smooth"))
        {
            value.SmoothTile (value.tileMap[(int)size.x, (int)size.y]);
        }
    }
}