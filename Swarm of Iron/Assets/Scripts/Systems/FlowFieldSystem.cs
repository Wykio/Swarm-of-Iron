using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace Swarm_Of_Iron_namespace
{
    public class FlowFieldSystem : JobComponentSystem
    {
        [ReadOnly] const int width = 100;
        [ReadOnly] const int height = 100;

        [ReadOnly] const int MAX_VALUE = 100;

        EntityQuery m_query, m_rock;

        public struct Neighbour
        {
            public int2 position;
            public int distance;
        }

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<MoveToComponent>());
            m_rock = GetEntityQuery(ComponentType.ReadOnly<RockComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle deps)
        {
            /* STEP 1 - Initialiser Dijkstra Grid */

            NativeArray<int> dijkstraGridBase = new NativeArray<int>(width * height, Allocator.TempJob);
            for (var i = 0; i < dijkstraGridBase.Length; i++) {
                dijkstraGridBase[i] = -1;
            }
            
            var job_initDijkstraGrid = new InitDijkstraGridJob()
            {
                m_maxValue = MAX_VALUE,
                m_width = width,
                m_height = height,
                m_dijkstraGrid = dijkstraGridBase
            };
            JobHandle deps1 = job_initDijkstraGrid.Schedule(m_rock, deps);


            EntityManager m_entityManager = Swarm_Of_Iron.instance.entityManager;

            List<TargetComponent> targets = new List<TargetComponent>();
            List<Int32> indices = new List<Int32>();
            m_entityManager.GetAllUniqueSharedComponentData<TargetComponent>(targets, indices);

            NativeArray<NativeArray<float2>> flowFields = new NativeArray<NativeArray<float2>>(targets.Count, Allocator.Temp);
            for (var idx = 0; idx < targets.Count; idx++)
            {
                int2 target = targets[idx].position;

                NativeArray<int> dijkstraGrid = new NativeArray<int>(dijkstraGridBase, Allocator.Temp);
                NativeArray<int2> neighbours = new NativeArray<int2>(4, Allocator.Temp);

                /* STEP 2 - Explore all node to construct Dijkstra Grid */

                //flood fill out from the end point
                Neighbour pathEnd = new Neighbour { position = target, distance = 0 };
                dijkstraGrid[target[0] + (target[1] * width)] = 0;

                int toVisitIndex = 0;
                NativeArray<Neighbour> toVisit = new NativeArray<Neighbour>(width * height, Allocator.Temp);
                toVisit[toVisitIndex++] = pathEnd;

                //for each node we need to visit, starting with the pathEnd
                for (var i = 0; i < toVisit.Length; i++)
                {
                    FlowFieldSystem.straightNeighboursOf(toVisit[i].position, width, neighbours);

                    //for each neighbour of this node (only straight line neighbours, not diagonals)
                    for (var j = 0; j < neighbours.Length; j++)
                    {
                        int2 n = neighbours[j];

                        //We will only ever visit every node once as we are always visiting nodes in the most efficient order
                        var dist = toVisit[i].distance + 1;
                        if (dijkstraGrid[n[0] + (n[1] * width)] == -1 || dijkstraGrid[n[0] + (n[1] * width)] > dist)
                        {
                            dijkstraGrid[n[0] + (n[1] * width)] = dist;
                            toVisit[toVisitIndex++] = new Neighbour { position = n, distance = dist };
                        }
                    }
                }
                toVisit.Dispose();

                /* STEP 3 - With Dijkstra Grid construct FlowField (array of dir vector) */

                NativeArray<float2> flowField = new NativeArray<float2>(width * height, Allocator.Temp);
                neighbours = new NativeArray<int2>(8, Allocator.Temp);

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        int index = x + y * width;
                        flowField[index] = new float2(0, 0);

                        //Obstacles have no flow value
                        if (dijkstraGrid[index] != MAX_VALUE)
                        {
                            int2 pos = new int2(x, y);
                            FlowFieldSystem.allNeighboursOf(pos, 0, 0, width, neighbours);

                            //Go through all neighbours and find the one with the lowest distance
                            int2 min = new int2(0, 0);
                            float minDist = MAX_VALUE;
                            for (var j = 0; j < neighbours.Length; j++)
                            {
                                int2 n = neighbours[j];
                                float dist = dijkstraGrid[n[0] + (n[1] * width)] - dijkstraGrid[index];

                                if (dist < minDist)
                                {
                                    min = n;
                                    minDist = dist;
                                }
                            }

                            //If we found a valid neighbour, point in its direction
                            if (minDist < MAX_VALUE)
                            {
                                flowField[index] = math.normalize(min - pos);
                            }
                        }
                    }
                }
                neighbours.Dispose();

                flowField[target[0] + (target[1] * width)] = new float2(0.0f, 0.0f);

                dijkstraGrid.Dispose();

                flowFields[indices[idx]] = flowField;
            }

            var mainJob = new UnitMovingJob() {
                deltatime = Time.DeltaTime,
                m_width = width,
                m_height = height,
                m_flowFields = flowFields
            };
            JobHandle finalDependency = mainJob.Schedule(m_query, deps1);
            
            return dijkstraGridBase.Dispose(finalDependency);
        }

        public static void straightNeighboursOf(int2 pos, int size, NativeArray<int2> res)
        {
            var index = 0;

            if (pos[0] > 0) res[index++] = new int2(pos[0] - 1, pos[1]);
            if (pos[1] > 0) res[index++] = new int2(pos[0], pos[1] - 1);

            if (pos[0] < size - 1) res[index++] = new int2(pos[0] + 1, pos[1]);
            if (pos[1] < size - 1) res[index++] = new int2(pos[0], pos[1] + 1);

            res = res.GetSubArray(0, index);
        }

        public static void allNeighboursOf(int2 pos, int top, int left, int size, NativeArray<int2> res)
        {
            var index = 0;

            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = pos[0] + dx;
                    var y = pos[1] + dy;

                    //All neighbours on the grid that aren't ourself
                    if (x >= top && y >= left && x < top + size && y < left + size && !(dx == 0 && dy == 0))
                    {
                        res[index++] = new int2(x, y);
                    }
                }
            }

            res = res.GetSubArray(0, index);
        }
    }
}