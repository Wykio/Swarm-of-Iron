using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

namespace SOI
{
    public static class SelectionMesh
    {
        public static Mesh CreateMesh()
        {
            return CreateMesh(2.0f, 2.0f);
        }

        public static Mesh CreateMesh(float meshWidth, float meshHeight)
        {
            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];

            float meshWidthHalf = meshWidth / 2f;
            float meshHeightHalf = meshHeight / 2f;

            vertices[0] = new Vector3(-meshWidthHalf, 0.0f, meshHeightHalf);
            vertices[1] = new Vector3(meshWidthHalf, 0.0f, meshHeightHalf);
            vertices[2] = new Vector3(-meshWidthHalf, 0.0f, -meshHeightHalf);
            vertices[3] = new Vector3(meshWidthHalf, 0.0f, -meshHeightHalf);

            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        public static void AddEntitySelectionMesh(Entity entityParent, bool isBuilding)
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(SelectionMeshComponent),
                typeof(LocalToWorld),
                typeof(LocalToParent),
                typeof(Parent),
                typeof(Translation),
                typeof(Scale),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Parent { Value = entityParent });
            if (!isBuilding)
            {
                entityManager.SetComponentData(entity, new Translation { Value = new float3(0.0f, -1.0f, 0.0f) });
                entityManager.SetComponentData(entity, new Scale { Value = 2.0f });
                entityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh = SwarmOfIron.Instance.unitSelectedCircleMesh,
                    material = SwarmOfIron.Instance.unitSelectedCircleMaterial
                });
            }
            else
            {
                entityManager.SetComponentData(entity, new Translation { Value = new float3(0.0f, 1.0f, 0.0f) });
                entityManager.SetComponentData(entity, new Scale { Value = 500.0f });
                entityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh = SwarmOfIron.Instance.unitSelectedCircleMesh,
                    material = SwarmOfIron.Instance.unitSelectedCircleMaterial
                });
            }
            
        }
    }
}
