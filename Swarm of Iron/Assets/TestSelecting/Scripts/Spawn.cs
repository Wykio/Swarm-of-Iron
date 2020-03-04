using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

public class Spawn : MonoBehaviour {

    public static Spawn instance;

    [SerializeField] private RenderMesh renderMesh;

    public Mesh unitSelectedMesh;
    public Material unitSelectedMaterial;

    private void Awake() {
        instance = this;
    }

    public void Start() {
        //On récupére l'entityManager
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //Création d'un archétype avec les "components"
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(PlayerInput),
            typeof(AABB)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(5, Allocator.Temp);

        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++) {
            Entity entity = entityArray[i];

            var position = new float3(UnityEngine.Random.Range(20.0f, 30.0f), 1.5f, UnityEngine.Random.Range(20.0f, 30.0f));
            // TODO: Eventually switch to the new Unity.Physics AABB 
            var aabb = new AABB {
                max = position + 0.5f,
                min = position - 0.5f,
            };

            entityManager.SetComponentData(entity, new PlayerInput());
            entityManager.SetComponentData(entity, new Translation { Value = position });
            entityManager.SetComponentData(entity, aabb);

            entityManager.SetSharedComponentData(entity, renderMesh);
        }
    }

}
