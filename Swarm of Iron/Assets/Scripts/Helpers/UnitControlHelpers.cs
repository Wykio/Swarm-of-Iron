using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics;
using Unity.Physics.Systems;

namespace SOI
{
    public static class UnitControlHelpers
    {
        public static float3 GetMousePosition() {
            return ScreenPointToWorldPoint(Input.mousePosition);
        }

        public static float3 ScreenPointToWorldPoint(float3 point) {
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(point);
            UnityEngine.RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit)) {
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

        public static Bounds GetViewportBounds(Camera camera, float3 screenPosition1, float3 screenPosition2) {
            var v1 = camera.ScreenToViewportPoint(screenPosition1);
            var v2 = camera.ScreenToViewportPoint(screenPosition2);
            var min = math.min(v1, v2);
            var max = math.max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static Entity Raycast(Vector3 _rayOrigin, Vector3 _to) {
            var physicWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorldSystem = physicWorldSystem.PhysicsWorld.CollisionWorld;

            var raycastInput = new Unity.Physics.RaycastInput {
                Start = _rayOrigin,
                End = _to,
                Filter = new CollisionFilter {
                    BelongsTo = ~0u,  // Everything
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            var raycatsHit = new Unity.Physics.RaycastHit();
            RaycastHelpers.SingleRaycast(collisionWorldSystem, raycastInput, ref raycatsHit);
              
            return raycatsHit.Entity;
        }

        public static Entity GetEntityTarget() {
            float3 _from = Input.mousePosition;
            float3 _to = ScreenPointToWorldPoint(_from);

            return Raycast(_from, _to);
        }
    }
}

