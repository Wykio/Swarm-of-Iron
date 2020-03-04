using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class PlayerUnitSelectSystem : JobComponentSystem {
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    // IJobForEachWithEntity play when to select entities
    struct PlayerUnitSelectJob : IJobForEachWithEntity<PlayerInput, AABB> {

        [NativeDisableParallelForRestriction] public EntityCommandBuffer CommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<PlayerUnitSelect> Selected;
        public Ray ray;

        public void Execute (Entity entity, int index, [ReadOnly] ref PlayerInput input, [ReadOnly] ref AABB aabb) {
            if (input.LeftClick) {
                if(Selected.Exists(entity)) {
                    CommandBuffer.RemoveComponent<PlayerUnitSelect>(entity);
                }

                // Add select component to unit
                if(RTSPhysics.Intersect(aabb, ray)) {
                    CommandBuffer.AddComponent(entity, new PlayerUnitSelect());
                }
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new PlayerUnitSelectJob {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            Selected = GetComponentDataFromEntity<PlayerUnitSelect>(),
            ray = Camera.main.ScreenPointToRay(Input.mousePosition),
        }.Schedule(this, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}