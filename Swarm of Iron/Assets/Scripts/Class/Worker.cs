using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace {
    public static class Worker {
      
        static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(UnitComponent),
                typeof(WorkerComponent),
                typeof(MoveToComponent),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new UnitComponent { animationSpeed = 0.5f });
            entityManager.SetComponentData(e, new MoveToComponent {
                move = false,
                moveSpeed = 10.0f
            });
            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = Swarm_Of_Iron.instance.workerMesh,
                material = Swarm_Of_Iron.instance.workerMaterial
            });
        }

    }
}

