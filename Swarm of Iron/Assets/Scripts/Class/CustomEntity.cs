using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;


namespace SOI {
    public static class CustomEntity {

        public static float3 GetRandomPosition(float sizeArea) {
            return new float3(UnityEngine.Random.Range(-sizeArea, sizeArea), 1.0f, UnityEngine.Random.Range(-sizeArea, sizeArea));
        }

        static public void SpawnEntityAtPosition(Type t, float3 spawnPosition) {
            SpawnEntitiesAtPosition(t, 1, spawnPosition);
        }

        static public void SpawnEntityAtRandomPosition(Type t) {
            SpawnEntitiesAtRandomPosition(t, 1);
        }

        static public void SpawnEntitiesAtPosition(Type t, int amout, float3 spawnPosition) {
            var GetArchetype = t.GetMethod("GetArchetype");
            var SetEntity = t.GetMethod("SetEntity");
            
            if (GetArchetype != null && SetEntity != null) {
                EntityManager entityManager = SwarmOfIron.Instance.entityManager;
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
                EntityManager entityManager = SwarmOfIron.Instance.entityManager;
                EntityArchetype entityArchetype = (EntityArchetype)GetArchetype.Invoke(null, null); //null - means calling static method

                NativeArray<Entity> entities = new NativeArray<Entity>(amout, Allocator.TempJob);
                entityManager.CreateEntity(entityArchetype, entities);

                float spawnAreaRange = SwarmOfIron.Instance.spawnAreaRange;

                var GetRandomPosition = t.GetMethod("GetRandomPosition");
                if (GetRandomPosition == null) GetRandomPosition = typeof(CustomEntity).GetMethod("GetRandomPosition");

                for (var i = 0; i < amout; i++) {
                    SetEntity.Invoke(null, new object[] { entities[i], GetRandomPosition.Invoke(null, new object[] { spawnAreaRange }) });
                }

                entities.Dispose();
            }
        }
    }
}

