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
        //instance du MonoBehaviour pour partager les attributs
        public static Swarm_Of_Iron instance;

        //EntityManager
        public EntityManager entityManager;

        [Header("Units Attributes")]
        //réferences pour les d'unitées
        public float spawnAreaRange = 50.0f; // taille de la zone de spawn

        [Header("Workers Attributes")]
        public Mesh workerMesh; // mesh pour les soldats
        public Material workerMaterial; // materiaux pour les soldats
        public int spawnWorkerAmount = 10; //nombre d'unité à spawn

        [Header("Soldiers Attributes")]
        public Mesh soldierMesh; // mesh pour les soldats
        public Material soldierMaterial; // materiaux pour les soldats
        public int spawnSoldierAmount = 10; //nombre d'unité à spawn

        [Header("Woods Attributes")]
        public Mesh leafMesh; // mesh pour les soldats
        public Material leafMaterial; // materiaux pour les soldats
        public Mesh trunkMesh; // mesh pour les soldats
        public Material trunkMaterial; // materiaux pour les soldats
        public int spawnWoodAmount = 1; //nombre d'unité à spawn

        [Header("Selection Attributes")]
        //réferences pour la selection d'unitée
        public Transform selectionAreaTransform;
        public Transform worldSelectionAreaTransform;
        public Mesh unitSelectedCircleMesh;
        public Material unitSelectedCircleMaterial;

        [Header("UI Attributes")]
        public GameObject houseCreationButton;
        public List<GameObject> listButtonGO;

        private UserInterface ui;

        private void Awake()
        {
            ui = new UserInterface(listButtonGO);

            //management des dépendences à revoir
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            //Init entityManager
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //create selection Mesh
            unitSelectedCircleMesh = SelectionMesh.CreateMesh();

            //spawn some woods
            Wood.SpawnWood(spawnWoodAmount);

            //spawn some workers
            Worker.SpawnWorkers(spawnWorkerAmount);

            //spawn some soldiers
            Soldier.init();
            Soldier.SpawnSoldiers(spawnSoldierAmount);
        }

        public UserInterface getUI() {
            return ui;
        }
    }
}
