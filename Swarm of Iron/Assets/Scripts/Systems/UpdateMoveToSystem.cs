using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

namespace SOI
{
    [UpdateInGroup(typeof(MoveLogicGroup))]
    [DisableAutoCreation]
    public class UpdateMoveToSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager m_entityManager = SwarmOfIron.Instance.entityManager;
            NativeArray<float3> movePositionList = new NativeArray<float3>(Soldier.movePositionList.ToArray(), Allocator.TempJob);

            Entity target = UnitControlHelpers.GetEntityTarget();
            float3 targetPosition = UnitControlHelpers.GetMousePosition();

            bool harvest = false;
            if (EntityManager.Exists(target))
            {
                harvest = EntityManager.HasComponent<RockComponent>(target);
            }

            EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            JobHandle jobHandle = Entities.WithAll<UnitSelectedComponent>().WithNone<CityHallComponent>().ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                entityCommandBuffer.AddComponent(entityInQueryIndex, entity, new MoveToComponent
                {
                    harvest = harvest,
                    startPosition = translation.Value,
                    endPosition = (!harvest ? movePositionList[entityInQueryIndex] : 0) + targetPosition
                });
            }).Schedule(inputDeps);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return movePositionList.Dispose(jobHandle);
        }
    }
}