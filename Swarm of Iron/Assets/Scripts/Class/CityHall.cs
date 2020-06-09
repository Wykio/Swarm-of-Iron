using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public static class CityHall
    {
        static public void SpawnCityHall()
        {
            float spawnAreaRange = Swarm_Of_Iron.instance.spawnAreaRange;
            SpawnCityHall(new float3(UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange), 1.0f, UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange)));
        }

        static public void SpawnCityHall(float3 spawnPosition)
        {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(CityHallComponent),
                typeof(UnitComponent),
                typeof(Translation),
                typeof(NonUniformScale),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetComponentData(entity, new NonUniformScale { Value = new float3(0.05f, 0, 0.05f) });
            entityManager.SetComponentData(entity, new CityHallComponent {
                ConstructionTime = 10.0f,
                ConstructionState = 0,
                LastConstructionStateTime = Time.deltaTime
            });

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.CityHallMesh,
                material = Swarm_Of_Iron.instance.CityHallMaterial
            });
        }
    }


}

