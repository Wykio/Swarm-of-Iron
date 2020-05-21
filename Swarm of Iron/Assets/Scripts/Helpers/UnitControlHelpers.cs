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
    }
}

