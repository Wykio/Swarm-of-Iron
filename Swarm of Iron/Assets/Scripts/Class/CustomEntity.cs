using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;


namespace Swarm_Of_Iron_namespace {
    public static class CustomEntity {

        public static float3 GetRandomPosition(float sizeArea) {
            return new float3(UnityEngine.Random.Range(-sizeArea, sizeArea), 1.0f, UnityEngine.Random.Range(-sizeArea, sizeArea));
        }

        static public void SpawnEntityAtPosition(Type t, float3 spawnPosition) {
            SpawnEntitiessAtPosition(t, 1, spawnPosition);
        }

        static public void SpawnEntityAtRandomPosition(Type t) {
            SpawnEntitiesAtRandomPosition(t, 1);
        }

        static public void SpawnEntitiessAtPosition(Type t, int amout, float3 spawnPosition) {
            var GetArchetype = t.GetMethod("GetArchetype");
            var SetEntity = t.GetMethod("SetEntity");
            
            if (GetArchetype != null && SetEntity != null) {
                EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
                EntityArchetype entityArchetype = (EntityArchetype)GetArchetype.Invoke(null, null); //null - means calling static method

                NativeArray<Entity> entities = new NativeArray<Entity>(amout, Allocator.TempJob);
                entityManager.CreateEntity(entityArchetype, entities);

                for (var i = 0; i < amout; i++) {
                    SetEntity.Invoke(null, new object[] { entities[i], spawnPosition });
                }

                entities.Dispose();
            }
        }

        static public void SpawnEntitiesAtRandomPosition(Type t, int amout) {
            var GetArchetype = t.GetMethod("GetArchetype");
            var SetEntity = t.GetMethod("SetEntity");
            
            if (GetArchetype != null && SetEntity != null) {
                EntityManager entityManager = Swarm_Of_Iron.instance.entityManager;
                EntityArchetype entityArchetype = (EntityArchetype)GetArchetype.Invoke(null, null); //null - means calling static method

                NativeArray<Entity> entities = new NativeArray<Entity>(amout, Allocator.TempJob);
                entityManager.CreateEntity(entityArchetype, entities);

                float spawnAreaRange = Swarm_Of_Iron.instance.spawnAreaRange;

                for (var i = 0; i < amout; i++) {
                    SetEntity.Invoke(null, new object[] { entities[i], GetRandomPosition(spawnAreaRange) });
                }

                entities.Dispose();
            }
        }
    }
}

