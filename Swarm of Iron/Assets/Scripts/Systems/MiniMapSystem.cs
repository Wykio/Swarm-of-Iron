using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;

namespace Swarm_Of_Iron_namespace
{
    public class MiniMapSystem : ComponentSystem
    {
        // TODO: faire correspondre au valeur réelle
        const int mapWidth = 500;
        const int mapHeight = 500;
        protected override void OnUpdate()
        {
            Image rend = Swarm_Of_Iron.instance.listButtonGO[1].GetComponent<Image>();

            // Create a texture
            Texture2D tex = new Texture2D(100, 100);
            Color[] colorArray = new Color[tex.width * tex.height];

            for (int x = 0; x < tex.width; x++) {
                for (int y = 0; y < tex.height; y++) {
                    colorArray[x + (y * tex.height)] = new Color(143.0f / 255.0f, 113.0f / 255.0f, 92.0f / 255.0f);
                }
            }

            Entities.WithAll<UnitComponent>().ForEach((ref Translation trans) => {
                int[] coords = ConvertWorldToTexture(trans.Value, tex);
                colorArray[coords[0] + (coords[1] * tex.height)] = Color.blue;
            });

            Entities.WithAll<WorkerComponent>().ForEach((ref Translation trans) => {
                int[] coords = ConvertWorldToTexture(trans.Value, tex);
                colorArray[coords[0] + (coords[1] * tex.height)] = Color.yellow;
            });

            int[,] outVect = new int[4,2] {
                {0, 0},
                {0, tex.height - 1},
                {tex.width - 1, tex.height - 1},
                {tex.width - 1, 0},
            };

            ConstructCameraCoordonates(outVect, tex);

            int length = outVect.GetLength(0);
            for (int i = 0; i < length; i++) {
                colorArray[outVect[i, 0] + (outVect[i, 1] * tex.height)] = Color.white;
                DrawLine(colorArray, tex, outVect[i, 0], outVect[i, 1], outVect[(i + 1) % length, 0], outVect[(i + 1) % length, 1], Color.white);
            }

            tex.SetPixels(colorArray);
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            // Create a sprite
            Sprite newSprite = Sprite.Create(tex, new Rect (0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            // Assign our procedural sprite
            rend.sprite = newSprite;
        }

        private int[] ConvertWorldToTexture(Vector3 vect, Texture2D tex) {
            // Enlever les chiffres negatif
            int transX = (int)vect.x + (mapWidth / 2);
            int transZ = (int)vect.z + (mapHeight / 2);

            // Convertir les coordonnées
            int x = (transX * tex.width) / mapWidth;
            int y = (transZ * tex.height) / mapHeight;

            return new int[2] {x, y};
        }

        private bool IsInMap (Vector3 vect) {
            int w = mapWidth / 2;
            int h = mapHeight / 2;
            return (-w < vect.x && vect.x < w && -h < vect.z && vect.z < h);
        }

        private void ConstructCameraCoordonates(int[,] outVect, Texture2D tex) {
            Ray ray;
            float distance;
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

            Vector3[] points = new Vector3[4] {
                new Vector3(0, 0, 0), // bottom left ray
                new Vector3(0, 1, 0), // top left ray
                new Vector3(1, 1, 0), // top right ray
                new Vector3(1, 0, 0), // botom right ray
            };
            
            for (int i = 0; i < 4; i++) {
                ray = Camera.main.ViewportPointToRay(points[i]);
                if (plane.Raycast(ray, out distance)) {
                    Vector3 vect = ray.GetPoint(distance);
                    if (IsInMap(vect)) {
                        int[] coords = ConvertWorldToTexture(vect, tex);
                        outVect[i, 0] = coords[0];
                        outVect[i, 1] = coords[1];
                    }
                }
            }
        }

        private void DrawLine(Color[] colorArray, Texture2D tex, int xA, int yA, int xB, int yB, Color color) {
            int xdiff = Mathf.Abs(xA - xB);
            int ydiff = Mathf.Abs(yA - yB);
            
            if (xdiff > ydiff) {
                for (int x = xA; x < xB; x++) {
                    int y = yB + (yA - yB) * (x - xB) / (xA - xB);
                    if (0 <= y && y < tex.height)
                        colorArray[x + (y * tex.height)] = color;
                }
            } else {
                for (int y = yA; y < yB; y++) {
                    int x = xB + (xA - xB) * (y - yB) / (yA - yB);
                    if (0 <= x && x < tex.width)
                        colorArray[x + (y * tex.height)] = color;
                }
            }
        }
    }
}

