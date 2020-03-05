using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public struct UnitSelected : IComponentData {
    }

    public class UnitSelectedRenderer : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<UnitSelected>().ForEach((ref Translation translation) => {
                //Graphics.DrawMesh();
            });
        }
    }

    public class UnitControlSystem : ComponentSystem
    {
        private float3 startPosition;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0)) {
                // Mouse Pressed
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(true);
                startPosition = getMousePosition();
                Swarm_Of_Iron.instance.selectionAreaTransform.position = startPosition;
                //Debug.Log(startPosition);
            }

            if (Input.GetMouseButton(0)) {
                // Mouse Held Down
                float3 selectionAeraSize = getMousePosition() - startPosition;
                Swarm_Of_Iron.instance.selectionAreaTransform.localScale = selectionAeraSize;
            }

            if (Input.GetMouseButtonUp(0)) {
                // Mouse Released
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false);
                float3 endPosition = getMousePosition();

                float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), 0.0f, math.min(startPosition.z, endPosition.z));
                float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), 0.0f, math.max(startPosition.z, endPosition.z));

                Entities.ForEach((Entity entity, ref Translation translation) => {
                    float3 entityPosition = translation.Value;

                    if (entityPosition.x >= lowerLeftPosition.x &&
                        entityPosition.z >= lowerLeftPosition.z &&
                        entityPosition.x <= upperRightPosition.x &&
                        entityPosition.z <= upperRightPosition.z) {
                        //Entity inside selection area
                        //Debug.Log(entity);
                        PostUpdateCommands.AddComponent(entity, new UnitSelected());
                    }  
                });
            }

        }

        private float3 getMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                return hit.point;
            }
            else {
                return -Vector3.one;
                Debug.Log("Click on Nothing !");
            }
        }
    }
}