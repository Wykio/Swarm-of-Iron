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
                Graphics.DrawMesh(
                    Swarm_Of_Iron.instance.unitSelectedCircleMesh,
                    translation.Value + new float3(0.0f, -1.0f, 0.0f), 
                    Quaternion.identity, 
                    Swarm_Of_Iron.instance.unitSelectedCircleMaterial, 
                    0);
            });
        }
    }

    public class UnitControlSystem : ComponentSystem
    {
        private float3 startPosition; //World Position
        private float3 startPositionScreen; //Screen Position

        protected override void OnUpdate()
        {
            // left click
            if (Input.GetMouseButtonDown(0)) {
                // Mouse Pressed
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(true);
                startPosition = getMousePosition(); //World Position
                startPositionScreen = Input.mousePosition; //Screen Position
                Swarm_Of_Iron.instance.selectionAreaTransform.position = startPositionScreen;
                //Debug.Log(startPositionScreen);
            }

            if (Input.GetMouseButton(0)) {
                // Mouse Held Down
                float3 currentPositionScreen = Input.mousePosition;//Screen Position
                float3 selectionAeraSize = currentPositionScreen - startPositionScreen;
                //Debug.Log(startPositionScreen + " -> " + currentPositionScreen);
                Swarm_Of_Iron.instance.selectionAreaTransform.localScale = selectionAeraSize;
                //Debug.Log(selectionAeraSize);
            }

            if (Input.GetMouseButtonUp(0)) {
                // Mouse Released
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false);
                float3 endPosition = getMousePosition();

                float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), 0.0f, math.min(startPosition.z, endPosition.z));
                float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), 0.0f, math.max(startPosition.z, endPosition.z));

                //Select OneUnitOnly
                bool selectOnlyOneEntity = false;

                //Click on OneUnitOnly
                float selectionAreaMinSize = 10.0f;
                float selectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);
                if (selectionAreaSize < selectionAreaMinSize) {
                    // SelectionArea too small
                    lowerLeftPosition += new float3(-1, 0, -1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                    upperRightPosition += new float3(+1, 0, +1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                    selectOnlyOneEntity = true;
                }

                // Deselect all Units
                Entities.WithAll<UnitSelected>().ForEach((Entity entity) => {
                    PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
                });

                // Select Units
                int selectedEntityCount = 0;
                Entities.ForEach((Entity entity, ref Translation translation) => {
                    if (selectOnlyOneEntity == false || selectedEntityCount < 1) {
                        float3 entityPosition = translation.Value;

                        if (entityPosition.x >= lowerLeftPosition.x &&
                            entityPosition.z >= lowerLeftPosition.z &&
                            entityPosition.x <= upperRightPosition.x &&
                            entityPosition.z <= upperRightPosition.z)
                        {
                            //Entity inside selection area
                            //Debug.Log(entity);
                            PostUpdateCommands.AddComponent(entity, new UnitSelected());
                            selectedEntityCount++;
                        }
                    }
                });
            }

            // right click
            if (Input.GetMouseButtonDown(1)) {
                Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveTo moveTo) => {
                    moveTo.position = getMousePosition();
                    moveTo.move = true;
                    //Debug.Log("move to" + moveTo.position);
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