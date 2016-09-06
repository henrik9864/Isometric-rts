using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureBaker
{

    public static Texture2D generateHighlight ( Texture2D toBake, int thickness, Color highlighColor )
    {
        Texture2D texture = new Texture2D (toBake.width, toBake.height);

        for (int x = 0; x < toBake.width; x++)
        {
            for (int y = 0; y < toBake.height; y++)
            {
                if (toBake.GetPixel (x, y).a == 0)
                {
                    texture.SetPixel (x, y, new Color (0, 0, 0, 0));
                    Color[] neighbours = getNeighbours (toBake, x, y, thickness);

                    foreach (Color pixel in neighbours)
                    {
                        if (pixel.a != 0)
                        {
                            texture.SetPixel (x, y, highlighColor);
                        }
                    }
                }
                else
                {
                    texture.SetPixel (x, y, toBake.GetPixel (x, y));
                }
            }
        }
        texture.Apply ();
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    static Color[] getNeighbours ( Texture2D texture, int startX, int startY, int range )
    {
        List<Color> neighbours = new List<Color> ();

        for (int x = -range; x < range + 1; x++)
        {
            for (int y = -range; y < range + 1; y++)
            {
                if (startX + x >= 0 && startX + x < texture.width && startY + y >= 0 && startY + y < texture.height && !(x == 0 && y == 0))
                {
                    neighbours.Add (texture.GetPixel (startX + x, startY + y));
                }
            }
        }

        return neighbours.ToArray ();
    }
}
