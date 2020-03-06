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

        public EntityManager entityManager;

        [SerializeField] private int spawnUnitAmount;
        public float spawnAreaRange;
        public Mesh soldierMesh;
        public Material soldierMaterial;

        //réferences pour la selection d'unitée
        public Transform selectionAreaTransform;
        public Mesh unitSelectedCircleMesh;
        public Material unitSelectedCircleMaterial;
        //public Mesh TestUnitSelectedCircleMesh;
        //public Material TestUnitSelectedCircleMaterial;

        private void Awake()
        {
            //management des dépendences à revoir
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Soldier.init();
            //create selection Mesh
            unitSelectedCircleMesh = SelectionMesh.CreateMesh(2.0f, 2.0f);
         
            // Spawn Soldiers
            for (int i = 0; i < spawnUnitAmount; i++) {
                Soldier.SpawnSoldier();
            }
        }
    }
}
