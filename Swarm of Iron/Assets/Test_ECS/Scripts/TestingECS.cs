using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

namespace testECS {
    public class TestingECS : MonoBehaviour
    {
        [SerializeField] private Mesh entityMesh;
        [SerializeField] private Material entityMaterial;

        public void Start()
        {
            //On récupére l'entityManager
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //Création d'un archétype avec les "components"
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(LevelComponent),
                typeof(MoveSpeedComponent),
                typeof(Translation),
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld)
                );

            //Création d'une entity d'un Archetype
            //Entity entity = entityManager.CreateEntity(entityArchetype);

            //Modification d'une entity
            //entityManager.SetComponentData(entity, new LevelComponent { level = 10 });

            //Création d'un nativeArray aloué temporairement car on utilise juste poue l'instanciation
            NativeArray<Entity> entityArray = new NativeArray<Entity>(10000, Allocator.Temp);

            //Création d'une entity avec un nativeArray
            entityManager.CreateEntity(entityArchetype, entityArray);

            for (int i = 0; i < entityArray.Length; i++)
            {
                Entity entity = entityArray[i];
                entityManager.SetComponentData(entity, new LevelComponent { level = UnityEngine.Random.Range(10.0f, 20.0f) });
                entityManager.SetComponentData(entity, new MoveSpeedComponent { moveSpeed = UnityEngine.Random.Range(1.0f, 2.0f) });
                entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-8.0f, 8.0f), UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) });

                entityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh = entityMesh,
                    material = entityMaterial,
                });
            }
        }

    }
}

