using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

namespace SOI {

    [UpdateAfter(typeof(UnitControlSystem))]
    public class UpdateMoveToSystem : JobComponentSystem {
        
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate() {
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            EntityManager m_entityManager = SwarmOfIron.Instance.entityManager;

            // TODO : Rendre les données accessible seuelement après un click et supprimer les données a la fin de ce job 
            Entity target = UnitControlHelpers.GetEntityTarget();
            float3 targetPosition = UnitControlHelpers.GetMousePosition();

            bool harvest = false;
            if (EntityManager.Exists(target)) {
                harvest = EntityManager.HasComponent<RockComponent>(target);
            }
            
            EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            JobHandle jobHandle = Entities.WithAll<UnitSelectedComponent>().WithNone<MoveToComponent>().ForEach((Entity entity, int entityInQueryIndex, in Translation translation) => {
                entityCommandBuffer.AddComponent(entityInQueryIndex, entity, new MoveToComponent {
                    harvest = harvest,
                    startPosition = translation.Value,
                    endPosition = targetPosition
                });
            }).Schedule(inputDeps);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}