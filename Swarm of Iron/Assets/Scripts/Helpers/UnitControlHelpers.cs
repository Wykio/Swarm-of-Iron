using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public static class UnitControlHelpers
    {
        public static float3 GetMousePosition() {
            return ScreenPointToWorldPoint(Input.mousePosition);
        }

        public static float3 ScreenPointToWorldPoint(float3 point) {
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                return hit.point;
            } else {
                return -Vector3.one;
                Debug.Log("Click on Nothing !");
            }
        }

        public static float3 MinArray(float3[] points) {
            if (points.Length == 0) return new float3();
            else if (points.Length == 1) return points[0];
            else {
                float3 tmp = new float3(math.min(points[0].x, points[1].x), 0.0f, math.min(points[0].z, points[1].z));
                for (var i = 2; i < points.Length; i++) {
                    tmp = new float3(math.min(tmp.x, points[i].x), 0.0f, math.min(tmp.z, points[i].z));
                }
                return tmp;
            }
        }

        public static float3 MaxArray(float3[] points) {
            if (points.Length == 0) return new float3();
            else if (points.Length == 1) return points[0];
            else {
                float3 tmp = new float3(math.max(points[0].x, points[1].x), 0.0f, math.max(points[0].z, points[1].z));
                for (var i = 2; i < points.Length; i++) {
                    tmp = new float3(math.max(tmp.x, points[i].x), 0.0f, math.max(tmp.z, points[i].z));
                }
                return tmp;
            }
        }
    }
}

