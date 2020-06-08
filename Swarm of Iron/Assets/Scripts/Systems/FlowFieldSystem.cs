using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Physics;

namespace SOI {
    [UpdateInGroup(typeof(MoveLogicGroup))]
    [UpdateAfter(typeof(UpdateMoveToSystem))]
    public class FlowFieldSystem : JobComponentSystem {
        private const int width = 50, height = 50, MAX_VALUE = 500;

        private EntityQuery _Obstacle;

        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate() {
            _Obstacle = GetEntityQuery(ComponentType.ReadOnly<PhysicsCollider>(), ComponentType.ReadOnly<Translation>());
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            /* STEP 1 - Initialiser Dijkstra Grid */

            NativeArray<int> dijkstraGridBase = new NativeArray<int>(width * height, Allocator.TempJob);
            JobHandle dependency = Dijkstra.Construct(dijkstraGridBase, width, height, MAX_VALUE, _Obstacle, inputDeps);

            EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            JobHandle jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<PathPosition> pathPositionBuffer, ref PathFollow pathFollow, in Translation translation, in MoveToComponent moveTo) => {
                if (!pathFollow.move) {
                    pathFollow.move = true;

                    int2 position = MiniMapHelpers.ConvertWorldCoord(translation.Value, width, height);
                    int2 target = MiniMapHelpers.ConvertWorldCoord(moveTo.endPosition, width, height);

                    /* STEP 2 - Explore all node to construct Dijkstra Grid */

                    NativeArray<int> dijkstraGrid = new NativeArray<int>(dijkstraGridBase.Length, Allocator.Temp);
                    dijkstraGrid.CopyFrom(dijkstraGridBase);

                    Dijkstra.Explore(dijkstraGrid, target, width, height);

                    /* STEP 3 - With Dijkstra Grid construct FlowField (array of dir vector) */

                    NativeArray<int2> flowfield = new NativeArray<int2>(width * height, Allocator.Temp);

                    Flowfield.Construct(flowfield, dijkstraGrid, position, target, width, height, MAX_VALUE);

                    pathPositionBuffer.CopyFrom(flowfield.Reinterpret<PathPosition>());

                    dijkstraGrid.Dispose();
                    flowfield.Dispose();

                    entityCommandBuffer.RemoveComponent<MoveToComponent>(entityInQueryIndex, entity);
                }
            }).Schedule(dependency);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return dijkstraGridBase.Dispose(jobHandle);
        }
    }
}