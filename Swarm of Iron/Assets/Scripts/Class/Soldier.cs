using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;


namespace SOI
{
    public static class Soldier
    {

        public static List<float3> movePositionList;

        //Initialise Positions Arrays for units
        static public void init()
        {
            float[] ringDistancesArray;
            int[] unitsPerRingArray;
            List<float> ringDistancesList = new List<float>();
            for (int i = 3; i < 3333; i += 3)
            {
                ringDistancesList.Add((float)i);
            }
            ringDistancesArray = ringDistancesList.ToArray();

            List<int> unitsPerRingList = new List<int>();
            for (int i = 8; i < 2500; i += 4)
            {
                unitsPerRingList.Add(i);
            }
            unitsPerRingArray = unitsPerRingList.ToArray();

            movePositionList = GetPositionListAround(new float3(0.0f, 0.0f, 0.0f), ringDistancesArray, unitsPerRingArray);
        }

        static public EntityArchetype GetArchetype()
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;
            return entityManager.CreateArchetype(
                typeof(UnitComponent),
                // typeof(PathPosition),
                // typeof(PathFollow),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderBounds)
            );
        }

        static public void SetEntity(Entity e, float3 position)
        {
            EntityManager entityManager = SwarmOfIron.Instance.entityManager;

            entityManager.SetComponentData(e, new Translation { Value = position });
            entityManager.SetComponentData(e, new UnitComponent { animationSpeed = 0.5f });
            // entityManager.SetComponentData(e, new PathFollow { move = false });
            entityManager.SetSharedComponentData(e, new RenderMesh
            {
                mesh = SwarmOfIron.Instance.soldierMesh,
                material = SwarmOfIron.Instance.soldierMaterial
            });
            entityManager.AddBuffer<PathPosition>(e);
        }

        // Fill position List for each rings
        static private List<float3> GetPositionListAround(float3 centerPosition, float[] ringDistance, int[] ringPositionCount)
        {
            List<float3> positionList = new List<float3>();
            positionList.Add(centerPosition);
            for (int ring = 0; ring < ringPositionCount.Length; ring++)
            {
                List<float3> ringPositionList = GetPositionListAround(centerPosition, ringDistance[ring], ringPositionCount[ring]);
                positionList.AddRange(ringPositionList);
            }
            return positionList;
        }

        // Get all positions for one ring
        static private List<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
        {
            List<float3> positionList = new List<float3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = i * (360 / positionCount);
                float3 direction = ApplyRotationToVector(new float3(1.0f, 0.0f, 0.0f), angle);
                float3 position = startPosition + direction * distance;
                positionList.Add(position);
            }
            return positionList;
        }

        // take a vector and apply an angle to it
        static private float3 ApplyRotationToVector(float3 vector, float angle)
        {
            return Quaternion.Euler(0.0f, angle, 0.0f) * vector;
        }
    }
}

