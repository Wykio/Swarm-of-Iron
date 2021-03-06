﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.UI;

namespace SOI
{
    public class SwarmOfIron : MonoBehaviour
    {
        //management des dépendences à revoir
        //instance du MonoBehaviour pour partager les attributs
        public static SwarmOfIron Instance { private set; get; }
        public UpdateMoveToSystem updateMoveToSystem;

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

        [Header("Enemie Soldiers Attributes")]
        public Mesh EsoldierMesh; // mesh pour les soldats
        public Material EsoldierMaterial; // materiaux pour les soldats
        public int EspawnSoldierAmount = 10; //nombre d'unité à spawn

        [Header("Projectiles Attributes")]
        public Mesh ProjectileMesh; // mesh pour les soldats
        public Material ProjectileMaterial; // materiaux pour les soldats

        [Header("Woods Attributes")]
        public Mesh leafMesh; // mesh pour les soldats
        public Material leafMaterial; // materiaux pour les soldats
        public Mesh trunkMesh; // mesh pour les soldats
        public Material trunkMaterial; // materiaux pour les soldats
        public int spawnWoodAmount = 1; //nombre d'unité à spawn

        [Header("Rock Attributes")]
        public Mesh rockMesh;
        public Material rockMaterial;
        public int spawnRockAmount = 1;

        [Header("CityHall Attributes")]
        public Mesh CityHallMesh; // mesh pour les soldats
        public Material CityHallMaterial; // materiaux pour les soldats
        public int CityHallConstructionTime;

        [Header("Selection Attributes")]
        //réferences pour la selection d'unitée
        public Transform selectionAreaTransform;
        public Transform worldSelectionAreaTransform;
        public GameObject selectionObj;
        public Mesh unitSelectedCircleMesh;
        public Material unitSelectedCircleMaterial;
        public GameObject cameraRig;
        public GameObject image;

        [Header("UI Attributes")]
        public List<GameObject> listButtonGO;
        public List<Texture2D> layers;
        public Text goldAmountText;
        public float goldAmount;

        private void Awake() {
            //management des dépendences à revoir
            Instance = this;
        }

        // Start is called before the first frame update
        private void Start() {
            DefaultWorldInitialization.Initialize("SwarmOfIron", false);
            
            World w = World.DefaultGameObjectInjectionWorld;
            //Init entityManager
            entityManager = w.EntityManager;
            updateMoveToSystem = w.GetOrCreateSystem<UpdateMoveToSystem>();

            //create selection Mesh
            unitSelectedCircleMesh = SelectionMesh.CreateMesh(1.0f, 1.0f);      

            //spawn some woods
            // Wood.SpawnWood(spawnWoodAmount);
            CustomEntity.SpawnEntitiesAtRandomPosition(typeof(Wood), spawnWoodAmount);
            CustomEntity.SpawnEntitiesAtRandomPosition(typeof(Rock), spawnRockAmount);

            //spawn some workers
            CustomEntity.SpawnEntitiesAtRandomPosition(typeof(Worker), spawnWorkerAmount);


            //spawn some soldiers
            Soldier.init();
            CustomEntity.SpawnEntitiesAtRandomPosition(typeof(Soldier), spawnSoldierAmount);
            //spawn Enemie soldiers
            E_Soldier.init();
            CustomEntity.SpawnEntitiesAtRandomPosition(typeof(E_Soldier), EspawnSoldierAmount);

            MiniMap.SpawnMiniMap();

            //CityHall.SpawnCityHall(new float3(0.0f, 0.0f, 0.0f));
        }

        private void Update()
        {
            //goldAmount++;
            goldAmountText.text = ((int)goldAmount).ToString();
        }

        public void ToggleSelectionArea (bool isActive) {
            this.selectionAreaTransform.gameObject.SetActive(isActive);
        }
    }
}
