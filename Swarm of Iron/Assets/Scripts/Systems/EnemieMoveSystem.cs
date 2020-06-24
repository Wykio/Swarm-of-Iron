using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;

namespace SOI
{
    [UpdateAfter(typeof(UnitAnimationSystem))]
    public class EnemieMoveSystem : ComponentSystem
    {
        private EntityQuery EnemiQuery, UnitQuery;
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        [BurstCompile]
        struct FindTarget : IJobForEachWithEntity<Translation>
        {
            [ReadOnly] public NativeArray<Translation> U_positions;
            public EntityCommandBuffer.Concurrent entityCommandBuffer;
            //public NativeArray<Translation> E_positions;
            

            public void Execute(Entity entity, int idxEntity, ref Translation translation)
            {
                float3 position = translation.Value;

                for (int i = 0; i < U_positions.Length; i++)
                {
                    if (math.distance(position, U_positions[i].Value) < 30f)
                    {
                        entityCommandBuffer.AddComponent(idxEntity, entity, new MoveToComponent
                        {
                            startPosition = translation.Value,
                            endPosition = U_positions[i].Value+1
                        });

                        if (math.distance(position, U_positions[i].Value) < 18f)
                        {
                            entityCommandBuffer.RemoveComponent<MoveToComponent>(idxEntity, entity);
                           // entityCommandBuffer.SpawnEntityAtPosition(typeof(Projectiles), translation.Value);


                        }

                        //translation.Value = U_positions[i].Value+1;
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadWrite<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
           // NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> AllUnitPos = UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new FindTarget()
            {
                U_positions = AllUnitPos,
                entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                //E_positions = AllEnemiPos
            };
            JobHandle dependency = job.Schedule(EnemiQuery);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(dependency);
            dependency.Complete();
            // AllEnemiPos.Dispose(dependency);
            AllUnitPos.Dispose(dependency);
        }


    }
}