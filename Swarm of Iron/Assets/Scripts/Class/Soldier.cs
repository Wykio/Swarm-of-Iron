using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;


namespace Swarm_Of_Iron_namespace
{
    public class Soldier
    {
        static public void SpawnSoldier()
        {
            float spawnAreaRange = Swarm_Of_Iron.instance.spawnAreaRange;
            SpawnSoldier(new float3(UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange), 1.0f, UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange)));
        }

        static public void SpawnSoldier(float3 spawnPosition)
        {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(SoldierComponent),
                typeof(MoveToComponent),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetComponentData(entity, new SoldierComponent { animationSpeed = 0.5f });
            entityManager.SetComponentData(entity, new MoveToComponent
            {
                move = false,
                moveSpeed = 10.0f
            });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.soldierMesh,
                material = Swarm_Of_Iron.instance.soldierMaterial
            });
        }
    }
}

