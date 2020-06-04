using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI {
    public static class Worker {
      
        static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(UnitComponent),
                typeof(WorkerComponent),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new UnitComponent { animationSpeed = 0.5f });
            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = SwarmOfIron.Instance.workerMesh,
                material = SwarmOfIron.Instance.workerMaterial
            });
        }

    }
}

