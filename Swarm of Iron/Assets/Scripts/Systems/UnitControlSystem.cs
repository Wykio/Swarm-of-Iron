using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public struct UnitSelected : IComponentData {
    }
    public struct SelectionMeshComponent : IComponentData
    {
    }

    public class UnitControlSystem : ComponentSystem
    {
        private float3 startPosition; //World Position
        private float3 startPositionScreen; //Screen Position

        protected override void OnUpdate()
        {
            // left click Down
            if (Input.GetMouseButtonDown(0)) {
                // Mouse Pressed
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(true);
                startPosition = getMousePosition(); //World Position
                startPositionScreen = Input.mousePosition; //Screen Position
                Swarm_Of_Iron.instance.selectionAreaTransform.position = startPositionScreen;
            }
            // Hold left click Down
            if (Input.GetMouseButton(0)) {
                // Mouse Held Down
                float3 currentPositionScreen = Input.mousePosition;//Screen Position
                float3 selectionAeraSize = currentPositionScreen - startPositionScreen; //Resize SCREEN selection Area
                Swarm_Of_Iron.instance.selectionAreaTransform.localScale = selectionAeraSize;
            }

            // left click Up
            if (Input.GetMouseButtonUp(0)) {
                // Mouse Released
                GetAllUnitsInSelectionArea();
            }

            // right click
            if (Input.GetMouseButtonDown(1)) {
                //move selected units
                moveAllUnitSelected();
            }
        }

        private void GetAllUnitsInSelectionArea()
        {
            Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false); //Desactivate SCREEN Selection Area
            float3 endPosition = getMousePosition();
            // Calculate WORLD selection area
            float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), 0.0f, math.min(startPosition.z, endPosition.z));
            float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), 0.0f, math.max(startPosition.z, endPosition.z));

            //Select OneUnitOnly
            bool selectOnlyOneEntity = false;

            //Click on OneUnitOnly
            float selectionAreaMinSize = 10.0f;
            float selectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);
            if (selectionAreaSize < selectionAreaMinSize)
            {
                // SelectionArea is too small => select only one unit
                lowerLeftPosition += new float3(-1, 0, -1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                upperRightPosition += new float3(+1, 0, +1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                selectOnlyOneEntity = true;
            }

            // Deselect all Units and Destroy all entities of the selection Mesh
            Entities.WithAll<UnitSelected>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
            });
            Entities.WithAll<SelectionMeshComponent>().ForEach((Entity entity) => {
                Swarm_Of_Iron.instance.entityManager.DestroyEntity(entity);
            });

            // Add "UnitSelected" component and create the entity for the selection Mesh
            int selectedEntityCount = 0;
            Entities.WithAll<SoldierComponent>().ForEach((Entity entity, ref Translation translation) => {
                if (selectOnlyOneEntity == false || selectedEntityCount < 1)
                {
                    float3 entityPosition = translation.Value;

                    if (entityPosition.x >= lowerLeftPosition.x &&
                        entityPosition.z >= lowerLeftPosition.z &&
                        entityPosition.x <= upperRightPosition.x &&
                        entityPosition.z <= upperRightPosition.z)
                    {
                        //Entity inside selection area
                        PostUpdateCommands.AddComponent(entity, new UnitSelected());
                        selectedEntityCount++;
                        AddEntitySelectionMesh(entity);
                    }
                }
            });
        }

        private void moveAllUnitSelected()
        {
            float3 targetPosition = getMousePosition();
            int positionIndex = 0;
            Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveToComponent moveTo) => {
                moveTo.move = true;
                moveTo.position = Soldier.movePositionList[positionIndex] + targetPosition;
                positionIndex = (positionIndex + 1) % Soldier.movePositionList.Count;
            });
        }
        
        private void AddEntitySelectionMesh(Entity entityParent)
        {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(SelectionMeshComponent),
                typeof(LocalToWorld),
                typeof(LocalToParent),
                typeof(Parent),
                typeof(Translation),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Parent { Value = entityParent });
            entityManager.SetComponentData(entity, new Translation { Value = new float3(0.0f, -1.0f, 0.0f) });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.unitSelectedCircleMesh,
                material = Swarm_Of_Iron.instance.unitSelectedCircleMaterial
            });
        }

        private float3 getMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }
            else
            {
                return -Vector3.one;
                Debug.Log("Click on Nothing !");
            }
        }
    }
}