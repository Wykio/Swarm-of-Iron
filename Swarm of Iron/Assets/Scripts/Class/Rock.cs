using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics;

namespace SOI
{
    public static class MeshExtensions
    {
        public static Mesh Copy(this Mesh mesh)
        {
            var copy = new Mesh();
            foreach (var property in typeof(Mesh).GetProperties())
            {
                if (property.GetSetMethod() != null && property.GetGetMethod() != null)
                {
                    property.SetValue(copy, property.GetValue(mesh, null), null);
                }
            }
            return copy;
        }
    }
}

namespace SOI
{
    static public class Rock
    {

        public static float3 GetRandomPosition(float sizeArea)
        {
            return new float3(UnityEngine.Random.Range(0, 10) * (sizeArea / 10) - (sizeArea / 2), 1.0f, UnityEngine.Random.Range(0, 10) * (sizeArea / 10) - (sizeArea / 2));
        }

        static public EntityArchetype GetArchetype()
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(RockComponent),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(PhysicsCollider)
            );
        }

        static public void SetEntity(Entity e, float3 position)
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;

            Mesh mesh = Rock.GenerateRockMesh();

            var mapSize = 500;
            var cellSize = 250; // 50;
            var scale = (mapSize / cellSize) / mesh.bounds.size.x;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new Rotation { Value = quaternion.EulerXYZ(new float3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f)) });
            entityManager.SetComponentData(e, new Scale { Value = scale });
            entityManager.SetSharedComponentData(e, new RenderMesh
            {
                mesh = mesh,
                material = SwarmOfIron.Instance.rockMaterial
            });

            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(
                new BoxGeometry
                {
                    Center = mesh.bounds.center,
                    Orientation = quaternion.identity,
                    Size = 5.0f,
                    BevelRadius = 0.0f
                });
            entityManager.SetComponentData(e, new PhysicsCollider { Value = collider });
        }

        static public Mesh GenerateRockMesh()
        {
            float offset = UnityEngine.Random.Range(0, 20);

            Mesh mesh = MeshExtensions.Copy(SwarmOfIron.Instance.rockMesh);

            Vector3[] vertices = mesh.vertices;
            List<Vector3> doneVerts = new List<Vector3>();
            for (int v = 0; v < vertices.Length; v++)
            {
                if (!doneVerts.Contains(vertices[v]))
                {
                    Vector3 curVector = vertices[v];
                    doneVerts.Add(curVector);

                    int smoothing = UnityEngine.Random.Range(2, 4);
                    Vector3 changedVector = (curVector + (Vector3.Normalize(curVector) * (Mathf.PerlinNoise((float)v / offset, (float)v / offset) / smoothing)));

                    for (int s = 0; s < vertices.Length; s++)
                    {
                        if (vertices[s] == curVector)
                        {
                            vertices[s] = changedVector;
                        }
                    }
                }
            }

            mesh.vertices = vertices;

            return mesh;
        }
    }
}