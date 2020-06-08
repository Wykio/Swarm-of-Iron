using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

namespace SOI
{
    [UpdateInGroup(typeof(MoveLogicGroup))]
    [UpdateAfter(typeof(FlowFieldSystem))]
    public class UnitMovingSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            JobHandle jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref MoveToComponent moveTo) =>
            {
                float reachedPositionDistance = 1.0f;
                float3 moveDir = math.normalize(moveTo.endPosition - translation.Value);
                float moveSpeed = 10f;

                translation.Value += moveDir * moveSpeed * deltaTime;

                if (math.distance(translation.Value, moveTo.endPosition) < reachedPositionDistance)
                {
                    if (moveTo.harvest)
                    {
                        var tmp = moveTo.startPosition;
                        moveTo.startPosition = moveTo.endPosition;
                        moveTo.endPosition = tmp;
                    }
                    else
                    {
                        entityCommandBuffer.RemoveComponent<MoveToComponent>(entityInQueryIndex, entity);
                    }
                }
            }).Schedule(inputDeps);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}