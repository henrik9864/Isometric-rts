using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CellularAutomata : MonoBehaviour
{

    public int simTime;
    public Vector2 mapSize;

    public List<CellBiome> biomes = new List<CellBiome> ();

    Cell[,] tileMap;

    //float timeToNextUpdate = 1;

    void Start ()
    {
        tileMap = new Cell[(int)mapSize.x, (int)mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                tileMap[x, y] = new Cell (x, y);

                int id = Random.Range (0, biomes.Count);
                tileMap[x, y].ChangeId (id, biomes[id].color);
            }
        }

        for (int i = 0; i < simTime; i++)
        {
            UpdateCells ();
        }
    }

    void Update ()
    {

    }

    void UpdateCells ()
    {
        Cell[,] newTileMap = new Cell[(int)mapSize.x, (int)mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                newTileMap[x, y] = tileMap[x, y].Clone ();
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Cell cell = tileMap[x, y];
                Cell newCell = newTileMap[x, y];

                for (int i = 0; i < biomes.Count; i++)
                {
                    List<Cell> neighbours = getNeighbours (x, y, 1, i);

                    if (neighbours.Count > 4 || (!newCell.alive && neighbours.Count > 0))
                    {
                        newCell.ChangeId (neighbours[0].id, neighbours[0].color);
                        break;
                    }
                    else if (neighbours.Count < 2 && newCell.id == i)
                    {
                        newCell.alive = false;
                    }
                }

                if (Random.Range (0f, 1f) < 0.005f)
                {
                    cell.alive = false;
                }
            }
        }

        print ("Update done");

        tileMap = newTileMap;
    }

    List<Cell> getNeighbours ( int startX, int startY, int id )
    {
        List<Cell> neighbours = new List<Cell> ();
        for (int x = -1; x < 2; x++)
        {
            if (startX + x >= 0 && startX + x < mapSize.x && x != 0)
            {
                if (tileMap[startX + x, startY] != null && tileMap[startX + x, startY].id == id)
                {
                    neighbours.Add (tileMap[startX + x, startY]);
                }
            }
        }

        for (int y = -1; y < 2; y++)
        {
            if (startY + y >= 0 && startY + y < mapSize.y && y != 0)
            {
                if (tileMap[startX, startY + y] != null && tileMap[startX, startY + y].id == id)
                {
                    neighbours.Add (tileMap[startX, startY + y]);
                }
            }
        }
        return neighbours;
    }

    List<Cell> getNeighbours ( int startX, int startY, int range, int id )
    {
        List<Cell> neighbours = new List<Cell> ();
        for (int x = -range; x < range + 1; x++)
        {
            for (int y = -range; y < range + 1; y++)
            {
                if (startX + x >= 0 && startX + x < mapSize.x && startY + y >= 0 && startY + y < mapSize.y && !(x == 0 && y == 0))
                {
                    if (tileMap[startX + x, startY] != null && (tileMap[startX + x, startY + y].id == id || !tileMap[startX + x, startY + y].alive))
                    {
                        neighbours.Add (tileMap[startX + x, startY + y]);
                    }
                }
            }
        }
        return neighbours;
    }

    void OnDrawGizmos ()
    {
        if (tileMap != null)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    Cell cell = tileMap[x, y];
                    Gizmos.color = cell.alive ? cell.color : Color.grey;
                    Gizmos.DrawWireCube (transform.position + new Vector3 (cell.pos.x, 0, cell.pos.y) - new Vector3 (mapSize.x, 0, mapSize.y) / 2, new Vector3 (1, .1f, 1) * .9f);
                }
            }
        }
    }
}

public class Cell
{
    public Vector2 pos;
    public bool alive;
    public int id;
    public Color color;

    public Cell ( int x, int y )
    {
        pos.x = x;
        pos.y = y;
    }

    public void ChangeId ( int newId, Color newColor )
    {
        id = newId;
        color = newColor;

        if (!alive)
            alive = true;
    }

    public void KillCell ()
    {
        alive = false;
        id = 0;
        color = new Color (0, 0, 0);
    }

    public Cell Clone ()
    {
        Cell cell = new Cell ((int)pos.x, (int)pos.y);
        cell.id = id;
        cell.alive = alive;
        cell.color = color;

        return cell;
    }
}

[System.Serializable]
public class CellBiome
{
    [HideInInspector]
    public string name = "Biome";

    public Color color;
}