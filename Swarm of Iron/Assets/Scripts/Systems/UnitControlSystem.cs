using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public class UnitControlSystem : ComponentSystem
    {
        //EntityQuery to optimise "component manipulation" in the entity manager
        private EntityQuery getAllUnitSelectedComponent;
        private EntityQuery getAllSelectionMeshComponent;

        private float3 startPosition; //World Position
        private float3 startPositionScreen; //Screen Position
        GameObject selectionObj; // debug

        private bool isUI = false;

        private List<Texture2D> layers;
        private int selectedEntityCount;
        private bool hasWorkerSelected;
        private string currentAction;

        protected override void OnUpdate() {
            // left click Down
            if (Input.GetMouseButtonDown(0)) {
                this.OnLeftClickDown();
            }

            // Hold left click Down
            if (Input.GetMouseButton(0)) {
                this.OnLeftClickMove();
            }

            if (this.currentAction == "HouseIcon") {
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.position = UnitControlHelpers.GetMousePosition() + new float3(-10, 1, 10);
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.localScale = new Vector3(20, 1, 20);
                Swarm_Of_Iron.instance.selectionAreaTransform.localScale = new Vector3(0, 0, 0);
            }

            // left click Up
            if (Input.GetMouseButtonUp(0) && !isUI) {
                var trans = Swarm_Of_Iron.instance.image.GetComponent<RectTransform>();
                Vector3[] v = new Vector3[4];
                trans.GetLocalCorners(v);

                deselectAllUnits();
                SelectedUnits(v, Swarm_Of_Iron.instance.worldSelectionAreaTransform.worldToLocalMatrix);
                // Mouse Released
                //GetAllUnitsInSelectionArea(startPosition, UnitControlHelpers.GetMousePosition());
                //GetAllUnitsInSelectionArea(startPositionScreen, Input.mousePosition);
            }

            // right click
            if (Input.GetMouseButtonDown(1)) {
                this.OnRightClickDown();
            }
        }

        private void OnLeftClickDown () {
            Swarm_Of_Iron.instance.ToggleSelectionArea(true);
            
            startPosition = UnitControlHelpers.GetMousePosition();  // World Position
            startPositionScreen = Input.mousePosition;              // Screen Position
            
            // selection OBJ Box
            selectionObj = Swarm_Of_Iron.instance.selectionObj;
            selectionObj.transform.position = startPosition;

            Vector3 cameraVec3 = Camera.main.transform.eulerAngles;
            selectionObj.transform.rotation = Quaternion.Euler(0.0f, cameraVec3.y, 0.0f);
            
            if (UserInterface.TryClickInterface(startPositionScreen, "Actions")) {
                // On a cliqué sur le menu des boutons d'actions
                isUI = true;

                // On récupère la coordonnée local du clique
                Vector3 localActionCoord = Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == "Actions").transform.InverseTransformPoint(startPositionScreen);

                // On récupère l'action & on met a jour
                this.currentAction = ActionHelpers.GetAction(localActionCoord, this.layers);
                ActionHelpers.UpdateActionUI(this.hasWorkerSelected, this.selectedEntityCount > 0, this.currentAction, ref this.layers);
            } else {
                isUI = false;

                Swarm_Of_Iron.instance.selectionAreaTransform.position = startPositionScreen;                       // Zone de sélection    rectangle vert      (screen)
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.position = startPosition + new float3(0, 1, 0);  // Debug                rectangle orange    (world)
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.rotation = Quaternion.Euler(-Camera.main.transform.eulerAngles.x, Swarm_Of_Iron.instance.cameraRig.transform.eulerAngles.y, 0);
            }
        }

        private void OnLeftClickMove () {
            float3 currentPositionScreen = Input.mousePosition;                     // Screen Position
            float3 currentPositionWorld = UnitControlHelpers.GetMousePosition();    // World Position

            float3 selectionAeraSize = currentPositionScreen - startPositionScreen; // Resize SCREEN selection Area
                                                                                    // Resize WORLD selection Area

            currentPositionWorld = RotatePointAroundPivot(currentPositionWorld, startPosition, Quaternion.Inverse(Camera.main.transform.rotation));
            //float3 worldSelectionAeraSize = currentPositionWorld - startPosition;

            //Vector3 cameraVec3 = Camera.main.transform.eulerAngles;

            //currentPositionWorld = RotatePointAroundPivot();
            float3 worldSelectionAeraSizetest = currentPositionWorld - startPosition;

            //Debug.Log("worldSelectionAeraSize = " + worldSelectionAeraSize + "worldSelectionAeraSizetest = " + worldSelectionAeraSizetest);
            //Debug.Log(worldSelectionAeraSize);
            Swarm_Of_Iron.instance.selectionAreaTransform.localScale = selectionAeraSize;
            Vector3 scaleSelectObj = new Vector3(worldSelectionAeraSizetest[0], 3.0f, worldSelectionAeraSizetest[2]);
            selectionObj.transform.localScale = scaleSelectObj;
            //Debug.Log(selectionObj.transform.localScale + " : " + selectionAeraSize + " : " + scaleSelectObj);

            // Debug 
            float3 endPosition = UnitControlHelpers.GetMousePosition();
            Swarm_Of_Iron.instance.worldSelectionAreaTransform.localScale = (endPosition - startPosition) * new float3(1, 1, -1);
        }

        private void OnRightClickDown() {
            ExecuteCurrentAction(currentAction);
        } 

        private void SelectedUnits(Vector3[] localCorners, Matrix4x4 worldToLocalMatrix) {
            float xrect = localCorners[1].x;
            float zrect = localCorners[1].y;

            float widthrect = math.abs(localCorners[3].x - localCorners[1].x);
            float heightrect = math.abs(localCorners[3].y - localCorners[1].y);

            Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) => {
                float3 localtrans = worldToLocalMatrix.MultiplyVector(translation.Value);
                
                // On execute une seule fois si l'unité a été séléctionnée directement
                float x = localtrans[0];
                float z = localtrans[2];

                if (xrect <= x && xrect + widthrect >= x && zrect <= z && zrect + heightrect >= z)
                {
                    Debug.Log(localtrans);
                    // Entity inside selection area
                    PostUpdateCommands.AddComponent(entity, new UnitSelectedComponent());
                    this.selectedEntityCount++;
                    SelectionMesh.AddEntitySelectionMesh(entity);

                    if (!this.hasWorkerSelected) {
                        this.hasWorkerSelected = EntityManager.HasComponent<WorkerComponent>(entity);
                    }
                }
            });
        }

        private void GetAllUnitsInSelectionArea(float3 startPos, float3 endPos) {
            Swarm_Of_Iron.instance.ToggleSelectionArea(false); //Desactivate SCREEN Selection Area

            // Calculate WORLD selection area
            float3 lowerLeftPosition = new float3(math.min(startPos.x, endPos.x), 0.0f, math.min(startPos.z, endPos.z));
            float3 upperRightPosition = new float3(math.max(startPos.x, endPos.x), 0.0f, math.max(startPos.z, endPos.z));
            /*float3 topLeft = new float3(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y), 0.0f);
            float3 botRight = new float3(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y), 0.0f);

            float3 lowerLeftPosition = ScreenPointToWorldPoint(topLeft);
            float3 upperRightPosition = ScreenPointToWorldPoint(botRight);*/

            //Select OneUnitOnly
            bool selectOnlyOneEntity = false;
            float selectionAreaMinSize = 10.0f;
            float selectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);
            if (selectionAreaSize < selectionAreaMinSize) {
                // SelectionArea is too small => select only one unit
                lowerLeftPosition += new float3(-1, 0, -1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                upperRightPosition += new float3(+1, 0, +1) * 0.2f * (selectionAreaMinSize - selectionAreaSize);
                selectOnlyOneEntity = true;
            }

            deselectAllUnits();

            selectAllUnits(selectOnlyOneEntity, lowerLeftPosition, upperRightPosition);
        }

        private void selectAllUnits(bool selectOnlyOneEntity, float3 lowerLeftPosition, float3 upperRightPosition) {
            // Add "UnitSelected" component and create the entity for the selection Mesh
            this.selectedEntityCount = 0;
            this.hasWorkerSelected = false;
            Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) => {
                // On execute une seule fois si l'unité a été séléctionnée directement
                if (selectOnlyOneEntity == false || selectedEntityCount < 1) {
                    /*float x = translation.Value.x;
                    float z = translation.Value.z;

                    float xrect = lowerLeftPosition.x;
                    float zrect = lowerLeftPosition.z;

                    float widthrect = upperRightPosition.x - lowerLeftPosition.x;
                    float heightrect = upperRightPosition.z - lowerLeftPosition.z;

                    if (xrect <= x && xrect + widthrect >= x && zrect <= z && zrect + heightrect >= z)
                    {*/
                    float3 entityPosition = translation.Value;

                    if (entityPosition.x >= lowerLeftPosition.x &&
                        entityPosition.z >= lowerLeftPosition.z &&
                        entityPosition.x <= upperRightPosition.x &&
                        entityPosition.z <= upperRightPosition.z)
                    {
                        //Entity inside selection area
                        PostUpdateCommands.AddComponent(entity, new UnitSelectedComponent());
                        this.selectedEntityCount++;
                        SelectionMesh.AddEntitySelectionMesh(entity);

                        if (!this.hasWorkerSelected) {
                            this.hasWorkerSelected = EntityManager.HasComponent<WorkerComponent>(entity);
                        }
                    }
                }
            });

            this.currentAction = "ArrowIcon";
            ActionHelpers.UpdateActionUI(this.hasWorkerSelected, this.selectedEntityCount > 0, this.currentAction, ref this.layers);
        }

        public void deselectAllUnits() {
            // Deselect all Units
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelectedComponent>(entity);
            });

            // Destroy all entities of the selection Mesh
            Entities.WithAll<SelectionMeshComponent>().ForEach((Entity entity) => {
                Swarm_Of_Iron.instance.entityManager.DestroyEntity(entity);
            });
        }

        public void moveAllUnitSelected() {
            float3 targetPosition = UnitControlHelpers.GetMousePosition();
            int positionIndex = 0;

            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity, ref MoveToComponent moveTo) => {
                moveTo.move = true;
                moveTo.position = Soldier.movePositionList[positionIndex] + targetPosition;
                positionIndex = (positionIndex + 1) % Soldier.movePositionList.Count;
            });
        }

        public void ExecuteCurrentAction(string action) {
            if (action == "ArrowIcon") {
                //move selected units
                moveAllUnitSelected();
            } else if (action == "HouseIcon") {
                CityHall.SpawnCityHall(UnitControlHelpers.GetMousePosition());
            }
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
            return rotation * (point - pivot) + pivot;
        }
    }
}