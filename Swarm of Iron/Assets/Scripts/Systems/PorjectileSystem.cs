using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;
/*
namespace SOI
{
    [UpdateAfter(typeof(UnitAnimationSystem))]
    public class PorjectileSystem : ComponentSystem
    {
        private EntityQuery EnemiQuery, UnitQuery, ProjectileQuery;
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        [BurstCompile]
        struct FindCible : IJobForEachWithEntity<Translation>
        {
            [ReadOnly] public NativeArray<Translation> U_positions;
            public EntityCommandBuffer.Concurrent entityCommandBuffer;
            //public NativeArray<Translation> E_positions;


            public void Execute(Entity entity, int idxEntity, ref Translation translation)
            {
                float3 position = translation.Value;

                for (int i = 0; i < U_positions.Length; i++)
                {
                    entityCommandBuffer.AddComponent(idxEntity, entity, new MoveToComponent
                    {
                        startPosition = translation.Value,
                        endPosition = U_positions[i].Value + 1
                    });

                }
            }
        }

        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadWrite<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
            ProjectileQuery = GetEntityQuery(ComponentType.ReadOnly<ProjectilesComponents>(), ComponentType.ReadWrite<Translation>());
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        }

        protected override void OnUpdate()
        {
            NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> AllUnitPos = UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);



            var job2 = new FindCible()
            {
                U_positions = AllUnitPos,
                entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            };
            JobHandle dependency2 = job.Schedule(ProjectileQuery);

        }
    }
}*/