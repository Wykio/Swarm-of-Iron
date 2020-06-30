using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

namespace SOI
{
    public class UnitControlSystem : ComponentSystem
    {
        private float3 startPosition; //World Position
        private float3 startPositionScreen; //Screen Position
        GameObject selectionObj; // debug

        private bool isUI = false;

        private List<Texture2D> layers;
        private int selectedEntityCount;
        private bool hasWorkerSelected;
        private bool hasHubSelected;
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
                SwarmOfIron.Instance.worldSelectionAreaTransform.position = UnitControlHelpers.GetMousePosition() + new float3(-10, 1, 10);
                SwarmOfIron.Instance.worldSelectionAreaTransform.localScale = new Vector3(20, 1, 20);
                SwarmOfIron.Instance.selectionAreaTransform.localScale = new Vector3(0, 0, 0);
            }
            

            // left click Up
            if (Input.GetMouseButtonUp(0) && !isUI) {
                this.OnLeftClickUp();
            }

            // right click
            if (Input.GetMouseButtonDown(1)) {
                this.OnRightClickDown();
            }
        }

        private void OnLeftClickDown () {
            SwarmOfIron.Instance.ToggleSelectionArea(true);
            
            startPosition = UnitControlHelpers.GetMousePosition();  // World Position
            startPositionScreen = Input.mousePosition;              // Screen Position
            
            // selection OBJ Box
            selectionObj = SwarmOfIron.Instance.selectionObj;
            selectionObj.transform.position = startPosition;

            Vector3 cameraVec3 = Camera.main.transform.eulerAngles;
            selectionObj.transform.rotation = Quaternion.Euler(0.0f, cameraVec3.y, 0.0f);
            
            if (UserInterface.TryClickInterface(startPositionScreen, "Actions")) {
                // On a cliqué sur le menu des boutons d'actions
                isUI = true;

                // On récupère la coordonnée local du clique
                Vector3 localActionCoord = SwarmOfIron.Instance.listButtonGO.Find(el => el.name == "Actions").transform.InverseTransformPoint(startPositionScreen);

                // On récupère l'action & on met a jour
                this.currentAction = ActionHelpers.GetAction(localActionCoord, this.layers);
                ActionHelpers.UpdateActionUI(this.hasHubSelected, this.hasWorkerSelected, this.selectedEntityCount > 0, this.currentAction, ref this.layers);

                if (hasHubSelected)
                {
                    //Debug.Log("hello PeonIcon");
                    spawWorkers();
                    //Worker.SpawnWorker(new float3(0, 0, 0));
                }

            } else {
                isUI = false;

                SwarmOfIron.Instance.selectionAreaTransform.position = startPositionScreen;                       // Zone de sélection    rectangle vert      (screen)
                SwarmOfIron.Instance.worldSelectionAreaTransform.rotation = Quaternion.Euler(0,  SwarmOfIron.Instance.cameraRig.transform.eulerAngles.y, 0);
            }
        }

        private void OnLeftClickMove () {
            float3 currentPositionScreen = Input.mousePosition;                     // Screen Position
            float3 currentPositionWorld = UnitControlHelpers.GetMousePosition();    // World Position

            float3 selectionAeraSize = currentPositionScreen - startPositionScreen; // Resize SCREEN selection Area
                                                                                    // Resize WORLD selection Area

            currentPositionWorld = RotatePointAroundPivot(currentPositionWorld, startPosition, Quaternion.Inverse(Camera.main.transform.rotation));
            //currentPositionWorld = RotatePointAroundPivot();
            float3 worldSelectionAeraSizetest = currentPositionWorld - startPosition;
            SwarmOfIron.Instance.selectionAreaTransform.localScale = selectionAeraSize;
            Vector3 scaleSelectObj = new Vector3(worldSelectionAeraSizetest[0], 3.0f, worldSelectionAeraSizetest[2]);
            selectionObj.transform.localScale = scaleSelectObj;
        }

        private void OnLeftClickUp() {
            SwarmOfIron.Instance.ToggleSelectionArea(false);

            this.selectedEntityCount = 0;
            this.hasWorkerSelected = false;
            this.hasHubSelected = false;
            this.currentAction = "ArrowIcon";

            DeselectAllUnits();
            SelectedUnits();
            ActionHelpers.UpdateActionUI(this.hasHubSelected, this.hasWorkerSelected, this.selectedEntityCount > 0, this.currentAction, ref this.layers);
        }

        private void OnRightClickDown() {
            ExecuteCurrentAction(currentAction);
        } 

        private void SelectedUnits() {
            // On parcours toutes les unitées
            Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) => {
                var viewportBounds = UnitControlHelpers.GetViewportBounds(Camera.main, this.startPositionScreen, Input.mousePosition);

                if (viewportBounds.Contains(Camera.main.WorldToViewportPoint(translation.Value)))
                {
                    // Entity inside selection area
                    PostUpdateCommands.AddComponent(entity, new UnitSelectedComponent());
                    this.selectedEntityCount++;

                    if (EntityManager.HasComponent<CityHallComponent>(entity))
                        SelectionMesh.AddEntitySelectionMesh(entity, true);                      
                    else
                        SelectionMesh.AddEntitySelectionMesh(entity, false);

                    if (!this.hasWorkerSelected) {
                        this.hasWorkerSelected = EntityManager.HasComponent<WorkerComponent>(entity);
                    }
                    if (!this.hasHubSelected) {
                        this.hasHubSelected = EntityManager.HasComponent<CityHallComponent>(entity);
                    }
                }
            });
        }

        public void DeselectAllUnits() {
            // Deselect all Units
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelectedComponent>(entity);
            });

            // Destroy all entities of the selection Mesh
            Entities.WithAll<SelectionMeshComponent>().ForEach((Entity entity) => {
                SwarmOfIron.Instance.entityManager.DestroyEntity(entity);
            });
        }

        public void ExecuteCurrentAction(string action) {
            if (action == "ArrowIcon") {
                //move selected units
                SwarmOfIron.Instance.updateMoveToSystem.Update();
            } else if (action == "HouseIcon") {
                if (SwarmOfIron.Instance.goldAmount >= 100.0f)
                {
                    SwarmOfIron.Instance.goldAmount -= 100.0f;
                    CustomEntity.SpawnEntityAtPosition(typeof(CityHall), UnitControlHelpers.GetMousePosition());
                }
            }
        }

        public void spawWorkers()
        {
            Entities.WithAll<CityHallComponent>().ForEach((ref Translation translation, ref UnitSelectedComponent unitSelectedComponent) => {
                if (SwarmOfIron.Instance.goldAmount >= 10.0f)
                {
                    SwarmOfIron.Instance.goldAmount -= 10.0f;
                    CustomEntity.SpawnEntityAtPosition(typeof(Worker), translation.Value + new float3(0, 0, -20));
                }
            });
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
            return rotation * (point - pivot) + pivot;
        }
    }
}