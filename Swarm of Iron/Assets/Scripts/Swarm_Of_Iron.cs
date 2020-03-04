using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public class Swarm_Of_Iron : MonoBehaviour
    {
        private EntityManager entityManager;

        [SerializeField] int spawnUnitAmount;
        [SerializeField] private Mesh soldierMesh;
        [SerializeField] private Material soldierMaterial;

        // Start is called before the first frame update
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            for (int i = 0; i < spawnUnitAmount; i++)
            {
                SpawnSoldier();
            }
        }

        private void SpawnSoldier()
        {
            SpawnSoldier(new float3(UnityEngine.Random.Range(-10.0f, 10.0f), 1.0f, UnityEngine.Random.Range(-10.0f, 10.0f)));
        }

        private void SpawnSoldier(float3 spawnPosition)
        {
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(Soldier),
                typeof(MoveTo),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = soldierMesh,
                material = soldierMaterial
            });
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPosition = -Vector3.one;

                //Method 1
                //clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0.0f , 0.0f, 10.0f));

                //Method 2
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    clickPosition = hit.point;
                }
                //Goal
                Debug.Log(clickPosition);
            }
        }
    }
}
