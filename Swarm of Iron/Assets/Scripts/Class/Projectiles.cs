using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;


namespace SOI
{
    public class Projectiles : MonoBehaviour
    {
        static public EntityArchetype GetArchetype()
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(ProjectilesComponents),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position)
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetSharedComponentData(e, new RenderMesh
            {
                mesh = SwarmOfIron.Instance.ProjectileMesh,
                material = SwarmOfIron.Instance.ProjectileMaterial
            });
        }
    }
}