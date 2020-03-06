using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Swarm_Of_Iron_namespace
{
    public class Swarm_Of_Iron : MonoBehaviour
    {
        //management des dépendences à revoir
        public static Swarm_Of_Iron instance;

        private EntityManager entityManager;

        [SerializeField] int spawnUnitAmount;
        [SerializeField] private Mesh soldierMesh;
        [SerializeField] private Material soldierMaterial;

        //réferences pour la selection d'unitée
        public Transform selectionAreaTransform;
        public Mesh unitSelectedCircleMesh;
        public Material unitSelectedCircleMaterial;

        private void Awake()
        {
            //management des dépendences à revoir
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            unitSelectedCircleMesh = Swarm_Of_Iron.CreateMesh(2.0f, 2.0f);

            for (int i = 0; i < spawnUnitAmount; i++)
            {
                SpawnSoldier();
            }
        }

        private void SpawnSoldier()
        {
            SpawnSoldier(new float3(UnityEngine.Random.Range(-10.0f, 10.0f), 1.0f, UnityEngine.Random.Range(-10.0f, 10.0f)));
        }

        private void SpawnSoldier(float3 spawnPosition)
        {
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(Soldier),
                typeof(MoveTo),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );

            Entity entity = entityManager.CreateEntity(entityArchetype);

            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetComponentData(entity, new Soldier { animationSpeed = 0.5f });
            entityManager.SetComponentData(entity, new MoveTo {
                move = false,
                moveSpeed = 10.0f
            });
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = soldierMesh,
                material = soldierMaterial
            });
        }

        //A déplacer + pas Opti j'ai l'impression
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
    }
}
