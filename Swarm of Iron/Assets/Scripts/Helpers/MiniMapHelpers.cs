using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI
{
    public static class MiniMapHelpers
    {
        // TODO: faire correspondre au valeur réelle
        const int mapWidth = 500;
        const int mapHeight = 500;

        public static int2 ConvertWorldCoord(float3 vect, int width, int height)
        {
            // Enlever les chiffres negatif
            int transX = (int)vect.x + (mapWidth / 2);
            int transZ = (int)vect.z + (mapHeight / 2);

            // Convertir les coordonnées
            int x = (transX * width) / mapWidth;
            int y = (transZ * height) / mapHeight;

            return new int2(x, y);
        }

        public static int2 ConvertWorldToTexture(float3 vect, int width, int height)
        {
            // Enlever les chiffres negatif
            int2 coords = ConvertWorldCoord(vect, width, height);

            return new int2(coords[0], height - 1 - coords[1]);
        }

        public static int2 DefineBounds(int2 coords, int width, int height)
        {
            coords[0] = math.clamp(coords[0], 0, width - 1);
            coords[1] = math.clamp(coords[1], 0, height - 1);

            return coords;
        }

        public static void ConstructCameraCoordonates(NativeArray<int2> outVect, int width, int height)
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
                    float3 vect = ray.GetPoint(distance);

                    int2 coords = ConvertWorldToTexture(vect, width, height);
                    coords = DefineBounds(coords, width, height);
                    outVect[i] = coords;
                }
            }
        }

        // Algorithme de tracé de segment de Bresenham (schooding tracé de ligne générique)
        public static void DrawLine(NativeArray<float4> colorArray, int width, int height, int xA, int yA, int xB, int yB, float4 color)
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

