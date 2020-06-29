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
        private float nextShootTime;
        private EntityQuery EnemiQuery, UnitQuery, ProjectileQuery;
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem, E_endSimulationEntityCommandBufferSystem;

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
                            entityCommandBuffer.RemoveComponent<ProjectilesComponents>(idxEntity, entity);
                            //PostUpdateCommands.RemoveComponent<UnitSelectedComponent>(entity);
                            //CustomEntity.SpawnEntityAtPosition(typeof(Projectiles), translation.Value);

                        }
                    }
                }
            }
        }

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

                    if (math.distance(position, U_positions[i].Value) < 3f)
                    {
                        entityCommandBuffer.RemoveComponent<MoveToComponent>(idxEntity, entity);
                    }

                }
            }
        }



        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadWrite<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
            ProjectileQuery = GetEntityQuery(ComponentType.ReadOnly<ProjectilesComponents>(), ComponentType.ReadWrite<Translation>());
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            E_endSimulationEntityCommandBufferSystem = endSimulationEntityCommandBufferSystem;
        }

        protected override void OnUpdate()
        {
            NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
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
            
            for (int i = 0; i < AllUnitPos.Length; i++)
            {
                for (int j = 0; j < AllEnemiPos.Length; j++)
                {
                 //   if (Time.DeltaTime > nextShootTime)
                   // {
                        if (math.distance(AllEnemiPos[j].Value, AllUnitPos[i].Value) < 18f)
                        {
                            CustomEntity.SpawnEntityAtPosition(typeof(Projectiles), AllEnemiPos[j].Value);
                            //float fireRate = .03f;
                            //nextShootTime = Time.DeltaTime + fireRate;
                        }
                   // }
                }
            }

            var job2 = new FindCible()
            {
                U_positions = AllUnitPos,
                entityCommandBuffer = E_endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            };
            JobHandle dependency2 = job2.Schedule(ProjectileQuery);

            E_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(dependency2);
            dependency2.Complete();
            AllUnitPos.Dispose(dependency2);
            AllEnemiPos.Dispose();
        }

    }
}