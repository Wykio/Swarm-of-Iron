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

            for (int i = 0; i < 4; i++) {
                int idx = (i + 1) % 4;
                DrawLine(colorArray, tex, outVect[i, 0], outVect[i, 1], outVect[idx, 0], outVect[idx, 1], Color.white);
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

        private void DefineBounds(int[] coords, Texture2D tex) {
            coords[0] = Mathf.Max(coords[0], 0);
            coords[1] = Mathf.Max(coords[1], 0);

            coords[0] = Mathf.Min(coords[0], tex.width - 1);
            coords[1] = Mathf.Min(coords[1], tex.height - 1);
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
                    
                    int[] coords = ConvertWorldToTexture(vect, tex);
                    DefineBounds(coords, tex);
                    outVect[i, 0] = coords[0];
                    outVect[i, 1] = coords[1];
                }
            }
        }

        private void DrawLine(Color[] colorArray, Texture2D tex, int xA, int yA, int xB, int yB, Color color) {
            int size = tex.width * tex.height;

            int xdiff = Mathf.Abs(xB - xA);
            int xsign = xA < xB ? 1 : -1;
            
            int ydiff = -Mathf.Abs(yB - yA);
            int ysign = yA < yB ? 1 : -1;
            
            int err = xdiff + ydiff;

            while (true) {
                int pos = xA + (yA * tex.height);
                if (0 <= pos && pos < size) colorArray[pos] = color;

                if (xA == xB && yA == yB) break;
                int e2 = 2 * err;

                if (e2 >= ydiff) {
                    err += ydiff;
                    xA += xsign;
                }

                if (e2 <= xdiff) {
                    err += xdiff;
                    yA += ysign;
                }            
            }
        }
    }
}

