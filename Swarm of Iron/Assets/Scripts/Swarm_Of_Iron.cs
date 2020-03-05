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
        //management des dépendences à revoir
        public static Swarm_Of_Iron instance;

        private EntityManager entityManager;

        [SerializeField] int spawnUnitAmount;
        [SerializeField] private Mesh soldierMesh;
        [SerializeField] private Material soldierMaterial;

        //réferences pour la selection d'unitée
        public Transform selectionAreaTransform;
        public Mesh unitSelectedCircleMesh;
        public Material unitSelectedCircleMaterial;

        private void Awake()
        {
            //management des dépendences à revoir
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //unitSelectedCircleMesh = Swarm_Of_Iron.createMesh
            //12.05

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
    }
}
