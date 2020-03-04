using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;

public class TestingDOTS : MonoBehaviour
{
    [SerializeField] private int entityAmount;
    [SerializeField] private Mesh entityMesh;
    [SerializeField] private Material entityMaterial;

    // Start is called before the first frame update
    private void Start()
    {
        //On récupére l'entityManager
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //Création d'un archétype avec les "components"
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
            );

        //Création d'un nativeArray aloué temporairement car on utilise juste poue l'instanciation
        NativeArray<Entity> entityArray = new NativeArray<Entity>(entityAmount, Allocator.Temp);

        //Création d'une entity avec un nativeArray
        entityManager.CreateEntity(entityArchetype, entityArray);

        //Setting des données des components des entities
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            //entityManager.SetComponentData(entity, new LevelComponent { level = UnityEngine.Random.Range(10.0f, 20.0f) });
            //entityManager.SetComponentData(entity, new MoveSpeedComponent { moveSpeed = UnityEngine.Random.Range(1.0f, 2.0f) });
            entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-8.0f, 8.0f), UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) });

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = entityMesh,
                material = entityMaterial,
            });
        }
    }
}
