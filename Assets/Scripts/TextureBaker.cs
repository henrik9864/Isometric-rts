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

    public static Texture2D blendTextures ( Texture2D topLeft, Texture2D topRight, Texture2D bottomLeft, Texture2D bottomRight, float precent )
    {
        Texture2D texture = new Texture2D (topLeft.width, topLeft.height);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float precentX = (float)x / texture.width;
                float precentY = (float)y / texture.height;

                Color bottomLeftBlend = topLeft.GetPixel (x, y) * ((1 - precentX) * (1 - precentY));
                Color bottomRightBlend = bottomRight.GetPixel (x, y) * ((precentX) * (1 - precentY));
                Color topLeftBlend = topLeft.GetPixel (x, y) * ((1 - precentX) * (precentY));
                Color topRightBlend = topRight.GetPixel (x, y) * ((precentX) * (precentY));

                Color color = bottomRightBlend + topLeftBlend + bottomLeftBlend + topRightBlend;

                texture.SetPixel (x, y, color);
            }
        }

        texture.Apply ();
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
