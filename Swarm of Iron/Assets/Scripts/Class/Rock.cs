using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics;

namespace Swarm_Of_Iron_namespace
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

namespace Swarm_Of_Iron_namespace {
    static public class Rock {

        static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(RockComponent),
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(PhysicsCollider)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            Mesh mesh = Rock.GenerateRockMesh();

            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new Rotation { Value = quaternion.EulerXYZ(new float3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f)) });
            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = mesh,
                material = Swarm_Of_Iron.instance.rockMaterial
            });

            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(
                new BoxGeometry {
                    Center = mesh.bounds.center,
                    Orientation = quaternion.identity,
                    Size = 5.0f,
                    BevelRadius = 0.0f
                });
            entityManager.SetComponentData(e, new PhysicsCollider { Value = collider });
        }

        static public Mesh GenerateRockMesh() {
            float offset = UnityEngine.Random.Range(0, 20);

            Mesh mesh = MeshExtensions.Copy(Swarm_Of_Iron.instance.sphereMesh);
            
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