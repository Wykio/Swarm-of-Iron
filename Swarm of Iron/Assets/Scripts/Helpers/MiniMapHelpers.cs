using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public static class MiniMapHelpers
    {
        // TODO: faire correspondre au valeur réelle
        const int mapWidth = 500;
        const int mapHeight = 500;

        public static int[] ConvertWorldToTexture(Vector3 vect, int width, int height)
        {
            // Enlever les chiffres negatif
            int transX = (int)vect.x + (mapWidth / 2);
            int transZ = (int)vect.z + (mapHeight / 2);

            // Convertir les coordonnées
            int x = (transX * width) / mapWidth;
            int y = (transZ * height) / mapHeight;

            return new int[2] { x, height -  1 - y };
        }

        public static void DefineBounds(int[] coords, int width, int height)
        {
            coords[0] = Mathf.Max(coords[0], 0);
            coords[1] = Mathf.Max(coords[1], 0);

            coords[0] = Mathf.Min(coords[0], width - 1);
            coords[1] = Mathf.Min(coords[1], height - 1);
        }

        public static void ConstructCameraCoordonates(int[,] outVect, int width, int height)
        {
            Ray ray;
            float distance;
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

            Vector3[] points = new Vector3[4] {
                new Vector3(0, 0, 0), // bottom left ray
                new Vector3(0, 1, 0), // top left ray
                new Vector3(1, 1, 0), // top right ray
                new Vector3(1, 0, 0), // botom right ray
            };

            for (int i = 0; i < 4; i++)
            {
                ray = Camera.main.ViewportPointToRay(points[i]);
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 vect = ray.GetPoint(distance);

                    int[] coords = ConvertWorldToTexture(vect, width, height);
                    DefineBounds(coords, width, height);
                    outVect[i, 0] = coords[0];
                    outVect[i, 1] = coords[1];
                }
            }
        }

        // Algorithme de tracé de segment de Bresenham (schooding tracé de ligne générique)
        public static void DrawLine(RenderTexture[] colorArray, int width, int height, int xA, int yA, int xB, int yB, RenderTexture color)
        {
            int size = width * height;

            int xdiff = Mathf.Abs(xB - xA);
            int xsign = xA < xB ? 1 : -1;

            int ydiff = -Mathf.Abs(yB - yA);
            int ysign = yA < yB ? 1 : -1;

            int err = xdiff + ydiff;

            while (true)
            {
                int pos = xA + (yA * height);
                if (0 <= pos && pos < size) colorArray[pos] = color;

                if (xA == xB && yA == yB) break;
                int e2 = 2 * err;

                if (e2 >= ydiff)
                {
                    err += ydiff;
                    xA += xsign;
                }

                if (e2 <= xdiff)
                {
                    err += xdiff;
                    yA += ysign;
                }
            }
        }
    }
}

