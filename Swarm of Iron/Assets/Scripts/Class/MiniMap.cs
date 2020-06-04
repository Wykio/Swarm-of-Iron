using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI
{
    public static class MiniMap
    {
        public static void SpawnMiniMap()
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(MiniMapComponent),
                typeof(RectComponent)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new RectComponent { x = 15, y = 15, width = 155, height = 155 });
            entityManager.AddBuffer<RenderTexture>(entity);
        }
    }
}

