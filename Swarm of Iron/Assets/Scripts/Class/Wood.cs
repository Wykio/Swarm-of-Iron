using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace {
    public static class Wood {

        static public EntityArchetype GetArchetype()  {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position) {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position + new float3(0.0f, -1.0f, 0.0f) });
            entityManager.SetComponentData(e, new Rotation { Value = quaternion.EulerXYZ(new float3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f)) });
            entityManager.SetSharedComponentData(e, new RenderMesh {
                mesh = Swarm_Of_Iron.instance.trunkMesh,
                material = Swarm_Of_Iron.instance.trunkMaterial
            });

            addLeaf(e);
        }

        static private void addLeaf(Entity entityParent)  {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(Parent),
                typeof(LocalToParent),
                typeof(Translation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity Leaf1entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(Leaf1entity, new Parent { Value = entityParent });
            entityManager.SetComponentData(Leaf1entity, new Translation { Value = new float3(0.68f, 9.66f, 3.3f) });
            entityManager.SetComponentData(Leaf1entity, new Scale { Value = 1.0f });
            entityManager.SetSharedComponentData(Leaf1entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.leafMesh,
                material = Swarm_Of_Iron.instance.leafMaterial
            });

            Entity Leaf2entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(Leaf2entity, new Parent { Value = entityParent });
            entityManager.SetComponentData(Leaf2entity, new Translation { Value = new float3(-0.17f, 9.56f, 0.19f) });
            entityManager.SetComponentData(Leaf2entity, new Scale { Value = 1.0f });
            entityManager.SetSharedComponentData(Leaf2entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.leafMesh,
                material = Swarm_Of_Iron.instance.leafMaterial
            });

            Entity Leaf3entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(Leaf3entity, new Parent { Value = entityParent });
            entityManager.SetComponentData(Leaf3entity, new Translation { Value = new float3(-1.4f, 5.87f, 0.65f) });
            entityManager.SetComponentData(Leaf3entity, new Scale { Value = 0.5f });
            entityManager.SetSharedComponentData(Leaf3entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.leafMesh,
                material = Swarm_Of_Iron.instance.leafMaterial
            });
        }
    }
}