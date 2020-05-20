using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
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

        protected override void OnUpdate() {
            // left click Down
            if (Input.GetMouseButtonDown(0)) {
                Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(true);
                startPosition = getMousePosition(); //World Position
                startPositionScreen = Input.mousePosition; //Screen Position
                //selection OBJ Box
                selectionObj = Swarm_Of_Iron.instance.selectionObj;
                selectionObj.transform.position = startPosition;
                Vector3 cameraVec3 = Camera.main.transform.eulerAngles;   
                selectionObj.transform.rotation = Quaternion.Euler(0.0f, cameraVec3.y, 0.0f);
                //
                if (UserInterface.TryClickInterface(startPositionScreen, "BuildHouseButton")) {
                    // siwtch UI state
                    isUI = !isUI;
                    if (!isUI) {
                        // construction is cancelled
                        Swarm_Of_Iron.instance.worldSelectionAreaTransform.localScale = new Vector3(0, 1, 0);
                        Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false);
                    }
                } else if (isUI) {
                    // TODO : callback
                    var constructPosition = startPosition;
                    isUI = false;
                    Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false);
                    CityHall.SpawnCityHall(constructPosition);
                } else {
                    // Debug 
                    Swarm_Of_Iron.instance.worldSelectionAreaTransform.position = startPosition + new float3(0, 1, 0);
                    Swarm_Of_Iron.instance.worldSelectionAreaTransform.rotation = Quaternion.Euler(0.0f, cameraVec3.y, 0.0f);

                    Swarm_Of_Iron.instance.selectionAreaTransform.position = startPositionScreen;
                }
            }

            // Hold left click Down
            if (Input.GetMouseButton(0) && !isUI) {
                float3 currentPositionScreen = Input.mousePosition;//Screen Position
                float3 currentPositionWorld = getMousePosition();//World Position

                float3 selectionAeraSize = currentPositionScreen - startPositionScreen; //Resize SCREEN selection Area
                //Resize WORLD selection Area

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
                float3 endPosition = getMousePosition();
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.localScale = (endPosition - startPosition) * new float3(1, 1, -1);
            }

            if (isUI) {
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.position = getMousePosition() + new float3(-10, 1, 10);
                Swarm_Of_Iron.instance.worldSelectionAreaTransform.localScale = new Vector3(20, 1, 20);
                Swarm_Of_Iron.instance.selectionAreaTransform.localScale = new Vector3(0, 0, 0);
            }

            // left click Up
            if (Input.GetMouseButtonUp(0)) {
                // Mouse Released
                if (!isUI)
                    GetAllUnitsInSelectionArea(startPosition, getMousePosition());
                    //GetAllUnitsInSelectionArea(startPositionScreen, Input.mousePosition);
            }

            // right click
            if (Input.GetMouseButtonDown(1)) {
                //move selected units
                moveAllUnitSelected();
            }

            float3 endPos = Input.mousePosition;
            float3 lowerLeftPosition = new float3(Mathf.Min(startPositionScreen.x, endPos.x), Mathf.Min(startPositionScreen.y, endPos.y), 0.0f);
            float3 upperRightPosition = new float3(Mathf.Max(startPositionScreen.x, endPos.x), Mathf.Max(startPositionScreen.y, endPos.y), 0.0f);
            
            Ray ray = Camera.main.ScreenPointToRay(lowerLeftPosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            
            ray = Camera.main.ScreenPointToRay(upperRightPosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);
        }

        private void GetAllUnitsInSelectionArea(float3 startPos, float3 endPos) {
            Swarm_Of_Iron.instance.selectionAreaTransform.gameObject.SetActive(false); //Desactivate SCREEN Selection Area

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

        private void deselectAllUnits() {
            // Deselect all Units and Destroy all entities of the selection Mesh
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelectedComponent>(entity);
            });
            //Swarm_Of_Iron.instance.entityManager.RemoveComponent(UnitSelectedComponent);
            Entities.WithAll<SelectionMeshComponent>().ForEach((Entity entity) => {
                Swarm_Of_Iron.instance.entityManager.DestroyEntity(entity);
            });
            //Desactive l'UI du worker
            Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == "BuildHouseButton").SetActive(false);

        }

        private void selectAllUnits(bool selectOnlyOneEntity, float3 lowerLeftPosition, float3 upperRightPosition) {
            // Add "UnitSelected" component and create the entity for the selection Mesh
            int selectedEntityCount = 0;
            bool hasWorkerSelected = false;
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
                        selectedEntityCount++;
                        AddEntitySelectionMesh(entity);

                        if (!hasWorkerSelected) {
                            hasWorkerSelected = EntityManager.HasComponent<WorkerComponent>(entity);
                        }
                    }
                }
            });

            Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == "BuildHouseButton").SetActive(hasWorkerSelected);
            
            this.UpdateActionUI(hasWorkerSelected, selectedEntityCount > 0);
            /*
            var componentDataFromEntity = GetComponentDataFromEntity<WorkerComponent, Unitselectedcomponent>();
            if (componentDataFromEntity.Exists(entity ?))
            {

            }*/
            /*
            //Degeulasse mais je sais pas faire autrement pour savoir si mon entity manager connait un "worker" qui est "selectionné"
            int selectedWorkerAmount = 0;
            Entities.ForEach((ref Translation translation, ref WorkerComponent workerComponent, ref UnitSelectedComponent unitSelectedComponent) =>
            {
                selectedWorkerAmount++;
            });
            if (selectedWorkerAmount > 0)
            {
                Debug.Log("Au moins un worker est selectionné");
            }*/
        }

        private void moveAllUnitSelected()  {
            float3 targetPosition = getMousePosition();
            int positionIndex = 0;
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity, ref MoveToComponent moveTo) => {
                moveTo.move = true;
                moveTo.position = Soldier.movePositionList[positionIndex] + targetPosition;
                positionIndex = (positionIndex + 1) % Soldier.movePositionList.Count;
            });
        }

        private void AddEntitySelectionMesh(Entity entityParent) {
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
            entityManager.SetSharedComponentData(entity, new RenderMesh {
                mesh = Swarm_Of_Iron.instance.unitSelectedCircleMesh,
                material = Swarm_Of_Iron.instance.unitSelectedCircleMaterial
            });
        }

        private float3 getMousePosition() {
            return this.ScreenPointToWorldPoint(Input.mousePosition);
        }

        private float3 ScreenPointToWorldPoint(float3 point) {
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                return hit.point;
            } else {
                return -Vector3.one;
                Debug.Log("Click on Nothing !");
            }
        }

        private void UpdateActionUI(bool hasWorkerSelected, bool hasSoldierSelected) {
            List<Texture2D> layers = new List<Texture2D>();
            for (int i = 0; i < Swarm_Of_Iron.instance.layers.Count; i++) {
                Texture2D texture = Swarm_Of_Iron.instance.layers[i];
                if (texture.name == "ArrowIcon.svg" && (hasWorkerSelected || hasSoldierSelected)) {
                    layers.Add(texture);
                } else if (texture.name == "HouseIcon.svg" && hasWorkerSelected) {
                    layers.Add(texture);
                }
            }

            Image rend = Swarm_Of_Iron.instance.listButtonGO.Find(el => el.name == "Actions").GetComponent<Image>();

            // Create a texture
            Texture2D tex = new Texture2D(32 * 3, 32 * 3);
            Color[] colorArray = new Color[tex.width * tex.height];
            Color[][] srcArray = new Color[layers.Count][];

            for (int i = 0; i < layers.Count; i++) {
                srcArray[i] = layers[i].GetPixels();
            }
            
            /* Redimensionnement de tout les layers */
            int dimx = tex.width / 3;
            int dimy = tex.height / 3; 

            int factorx = tex.width / dimx;
            int factory = tex.height / dimy;
            
            int actionsCount = factorx * factory;

            int x = 0, y = 0;
            for (int idx = 0; idx < layers.Count && idx < actionsCount; idx++) {
                for (int i = 0; i < dimx; i++) {
                    for (int j = 0; j < dimy; j++) {
                        // tex.height - y car dans Unity l'origine d'une image est en bas a gauche
                        int pixelIndex = x + ((tex.height - y) * tex.width);
                        // pareil pour layers[idx].height - j
                        int scrIdx = i + (layers[idx].height - j) * layers[idx].width;
                        if (scrIdx < layers[idx].width * layers[idx].height) {
                            Color srcPixel = srcArray[idx][scrIdx];
                            if (srcPixel.a == 1) {
                                colorArray[pixelIndex] = srcPixel;
                            }
                        }
                        int tmp = (int)Mathf.Floor(idx / factorx);
                        if (y == dimy * (tmp + 1) - 1) {
                            x++;
                            y = dimy * tmp;
                        } else {
                            y++;
                        }
                    }
                    if (x == tex.width - 1) {
                        x = 0;
                    }
                }
            }

            tex.SetPixels(colorArray);
            tex.Apply();

            tex.wrapMode = TextureWrapMode.Clamp;

            // Create a sprite
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            // Assign our procedural sprite
            rend.sprite = newSprite;
        }

        //Don't use this, it completely crashes the game for no reason oO ... Florian can you give a look ?
        /*
        protected override void OnCreate()
        {
            //getAllUnitSelectedComponent = GetEntityQuery(typeof(UnitSelectedComponent));
            //getAllSelectionMeshComponent = GetEntityQuery(typeof(SelectionMeshComponent));
            var query = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(UnitSelectedComponent) }
            };
            getAllUnitSelectedComponent = GetEntityQuery(query);
        }*/

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
    }
}