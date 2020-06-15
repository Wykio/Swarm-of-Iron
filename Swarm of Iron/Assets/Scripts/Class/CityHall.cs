using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI {
    public static class CityHall  {

      static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(CityHallComponent),
                typeof(UnitComponent),
                typeof(Translation),
                typeof(NonUniformScale),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new NonUniformScale { Value = new float3(0.05f, 0, 0.05f) });
            entityManager.SetComponentData(e, new CityHallComponent {
                ConstructionTime = 10.0f,
                ConstructionState = 0,
                LastConstructionStateTime = Time.deltaTime
            });

            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = SwarmOfIron.Instance.CityHallMesh,
                material = SwarmOfIron.Instance.CityHallMaterial
            });
        }
    }
}

