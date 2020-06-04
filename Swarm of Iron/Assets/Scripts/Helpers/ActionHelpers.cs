using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI
{
    public static class ActionHelpers
    {
        public static void UpdateActionUI(bool hasWorkerSelected, bool hasSoldierSelected, string action, ref List<Texture2D> layers)
        {
            int actionIdx = -1;

            layers = new List<Texture2D>();
            for (int i = 0; i < SwarmOfIron.Instance.layers.Count; i++)
            {
                Texture2D texture = SwarmOfIron.Instance.layers[i];
                if (texture.name == "ArrowIcon" && (hasWorkerSelected || hasSoldierSelected))
                {
                    layers.Add(texture);
                    if (action == "ArrowIcon") actionIdx = i;
                }
                else if (texture.name == "HouseIcon" && hasWorkerSelected)
                {
                    layers.Add(texture);
                    if (action == "HouseIcon") actionIdx = i;
                }
            }

            Image rend = SwarmOfIron.Instance.listButtonGO.Find(el => el.name == "Actions").GetComponent<Image>();

            // Create a texture
            Texture2D tex = new Texture2D(32 * 3, 32 * 3);
            Color[] colorArray = new Color[tex.width * tex.height];

            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    int pixelIndex = GetPixelIndex(i, j, tex.width, tex.height);
                    colorArray[pixelIndex] = Color.white;
                }
            }

            Color[][] srcArray = new Color[layers.Count][];

            for (int i = 0; i < layers.Count; i++)
            {
                srcArray[i] = layers[i].GetPixels();
            }

            int dimx = tex.width / 3;
            int dimy = tex.height / 3;

            int factorx = tex.width / dimx;
            int factory = tex.height / dimy;

            int actionsCount = factorx * factory;

            int x = 0, y = 0;
            for (int idx = 0; idx < layers.Count && idx < actionsCount; idx++)
            {
                int scaley = (int)Mathf.Floor(actionIdx / factory);

                for (int i = 0; i < dimx; i++)
                {
                    for (int j = 0; j < dimy; j++)
                    {
                        // tex.height - y car dans Unity l'origine d'une image est en bas a gauche
                        int pixelIndex = GetPixelIndex(x, y, tex.width, tex.height);
                        // pareil pour layers[idx].height - j
                        int scrIdx = GetPixelIndex(i, j, layers[idx].width, layers[idx].height);

                        if (scrIdx < layers[idx].width * layers[idx].height)
                        {
                            Color srcPixel = srcArray[idx][scrIdx];
                            if (srcPixel.a == 1)
                            {
                                colorArray[pixelIndex] = srcPixel;
                            }
                        }

                        if (y == dimy * (scaley + 1) - 1)
                        {
                            x++;
                            y = dimy * scaley;
                        }
                        else
                        {
                            y++;
                        }
                    }
                    if (x == tex.width - 1)
                    {
                        x = 0;
                    }
                }
            }

            // Encadrer l'action courante
            if (0 <= actionIdx && actionIdx < 9)
            {
                int scalex = actionIdx % factorx;
                int scaley = (int)Mathf.Floor(actionIdx / factory);

                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        int pixelIndex = GetPixelIndex(i, j, tex.width, tex.height);

                        float minx = dimx * scalex;
                        float miny = dimy * scaley;
                        float maxx = dimx * (scalex + 1) - 1;
                        float maxy = dimy * (scaley + 1) - 1;

                        if (i >= minx && j >= miny && i <= maxx && j <= maxy)
                            if (i == minx || j == miny || i == maxx || j == maxy)
                                colorArray[pixelIndex] = Color.green;
                    }
                }
            }

            tex.SetPixels(colorArray);
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            // Create a sprite
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            // Assign our procedural sprite
            rend.sprite = newSprite;
        }

        private static int GetPixelIndex(int x, int y, int width, int height)
        {
            return x + (((height - 1) - y) * width);
        }

        public static string GetAction(Vector3 pos, List<Texture2D> layers)
        {
            for (var i = 0; i < layers.Count; i++)
            {
                int scalex = i % 3;
                int scaley = (int)Mathf.Floor(i / 3);

                scaley = (scaley == 0) ? 2 : (scaley == 2) ? 0 : 1;

                float minx = 32 * scalex;
                float miny = 32 * scaley;
                float maxx = 32 * (scalex + 1) - 1;
                float maxy = 32 * (scaley + 1) - 1;

                if (pos.x >= minx && pos.y >= miny && pos.x <= maxx && pos.y <= maxy)
                {
                    return layers[i].name;
                }
            }
            return "";
        }
    }
}

