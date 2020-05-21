using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public static class Worker
    {
        static public bool isWorkerSelected;

        static public void InitWorker()
        {
            isWorkerSelected = false;
        }

        static public void SpawnWorker()
        {
            float spawnAreaRange = Swarm_Of_Iron.instance.spawnAreaRange;
            SpawnWorker(new float3(UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange), 1.0f, UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange)));
        }

        static public void SpawnWorkers(int amount)
        {
            // Spawn Soldiers
            for (int i = 0; i < amount; i++)
            {
                SpawnWorker();
            }
        }

        static public void SpawnWorker(float3 spawnPosition)
        {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(UnitComponent),
                typeof(WorkerComponent),
                typeof(MoveToComponent),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetComponentData(entity, new UnitComponent { animationSpeed = 0.5f });
            entityManager.SetComponentData(entity, new MoveToComponent
            {
                move = false,
                moveSpeed = 10.0f
            });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.workerMesh,
                material = Swarm_Of_Iron.instance.workerMaterial
            });
        }

    }
}

