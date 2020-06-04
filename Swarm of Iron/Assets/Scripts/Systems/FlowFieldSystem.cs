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
            m_query = GetEntityQuery(ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<MoveToComponent>(), ComponentType.ReadOnly<TargetComponent>());
            m_rock = GetEntityQuery(ComponentType.ReadOnly<RockComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle dependency)
        {
            /* STEP 1 - Initialiser Dijkstra Grid */

            NativeArray<int> dijkstraGridBase = new NativeArray<int>(width * height, Allocator.TempJob);
            dependency = DijkstraHelper.Construct(dijkstraGridBase, m_rock, dependency, width, height, MAX_VALUE);
            dependency.Complete();

            List<TargetComponent> targets = new List<TargetComponent>();
            EntityManager.GetAllUniqueSharedComponentData<TargetComponent>(targets);

            for (var idx = 0; idx < targets.Count; idx++) {
                int2 target = targets[idx].position;

                /* STEP 2 - Explore all node to construct Dijkstra Grid */

                NativeArray<int> dijkstraGrid = new NativeArray<int>(dijkstraGridBase, Allocator.TempJob);
                DijkstraHelper.Explore(dijkstraGrid, target, width, height);

                /* STEP 3 - With Dijkstra Grid construct FlowField (array of dir vector) */

                NativeArray<float2> flowField = new NativeArray<float2>(width * height, Allocator.TempJob);
                FlowFieldHelper.Construct(flowField, dijkstraGrid, width, height, MAX_VALUE);
                flowField[target[0] + (target[1] * width)] = new float2(0.0f, 0.0f);

                m_query.SetSharedComponentFilter<TargetComponent>(targets[idx]);
                dependency = (new UnitMovingJob() {
                    deltatime = Time.DeltaTime,
                    m_width = width,
                    m_height = height,
                    m_flowFields = flowField
                }).Schedule(m_query, dependency);

                dependency = dijkstraGrid.Dispose(dependency);
                dependency = flowField.Dispose(dependency);
            }

            dependency = dijkstraGridBase.Dispose(dependency);
            
            return dependency;
        }
    }
}