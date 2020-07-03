using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

namespace SOI
{
    [UpdateInGroup(typeof(MoveLogicGroup))]
    [UpdateAfter(typeof(FlowFieldSystem))]
    public class projectileMovingSystem : JobComponentSystem
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

            JobHandle jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref ProMoveComponents ProjmoveTo) =>
            {
                // Ignore Y coordinate
                float3 start = translation.Value;
                float3 end = ProjmoveTo.endPosition;
                start.y = 0.0f;
                end.y = 0.0f;

                float reachedPositionDistance = 1.0f;
                float3 moveDir = math.normalize(end - start);
                float moveSpeed = 20f;

                translation.Value += moveDir * moveSpeed * deltaTime;

                if (math.distance(start, end) < reachedPositionDistance)
                {
                        entityCommandBuffer.RemoveComponent<ProMoveComponents>(entityInQueryIndex, entity);
                    
                }
            }).Schedule(inputDeps);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}
