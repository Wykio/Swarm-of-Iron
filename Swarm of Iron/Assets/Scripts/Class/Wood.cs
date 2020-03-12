using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public class Wood
    {
        static public void SpawnWood()
        {
            float spawnAreaRange = Swarm_Of_Iron.instance.spawnAreaRange;
            SpawnWood(new float3(UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange), 1.0f, UnityEngine.Random.Range(-spawnAreaRange, spawnAreaRange)));
        }

        static public void SpawnWood(int Amount)
        {
            // Spawn Woods
            for (int i = 0; i < Swarm_Of_Iron.instance.spawnWoodAmount; i++)
            {
                SpawnWood();
            }
        }

        static public void SpawnWood(float3 spawnPosition)
        {
            EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition + new float3(0.0f, -1.0f, 0.0f)});
            //entityManager.SetComponentData(entity, new RotationEulerXYZ { Value = new float3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) });
            //entityManager.SetComponentData(entity, new RotationEulerXYZ { Value = new float3(0.0f, 0.0f, 0.0f) });
            quaternion quaternion = new quaternion();
            entityManager.SetComponentData(entity, new Rotation { Value = quaternion.EulerXYZ(new float3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f)) });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Swarm_Of_Iron.instance.trunkMesh,
                material = Swarm_Of_Iron.instance.trunkMaterial
            });

            addLeaf(entity);
        }

        static private void addLeaf(Entity entityParent)
        {
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