using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace {
    public static class CityHall  {

      static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(CityHallComponent),
                typeof(Translation),
                typeof(NonUniformScale),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new NonUniformScale { Value = new float3(0.05f, 0, 0.05f) });
            entityManager.SetComponentData(e, new CityHallComponent {
                ConstructionTime = 10.0f,
                ConstructionState = 0,
                LastConstructionStateTime = Time.deltaTime
            });

            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = Swarm_Of_Iron.instance.CityHallMesh,
                material = Swarm_Of_Iron.instance.CityHallMaterial
            });
        }
    }
}

