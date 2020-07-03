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
    public class PorjectileSystem : ComponentSystem
    {
        float currentTime = 0f;
        private float nextShootTime;
        private EntityQuery EnemiQuery, UnitQuery, ProjectileQuery;
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        //For the projectile Find Enemies and arrive to the enemie.(Job)
        [BurstCompile]
        struct FindCible : IJobForEachWithEntity<Translation>
        {
            [ReadOnly] public NativeArray<Translation> U_positions;
            public EntityCommandBuffer.Concurrent entityCommandBuffer;

            public void Execute(Entity entity, int idxEntity, ref Translation translation)
            {
                float3 position = translation.Value;
                for (int i = 0; i < U_positions.Length; i++)
                {
                    //CustomEntity.SpawnEntityAtPosition(typeof(Projectiles), translation.Value);
                    entityCommandBuffer.AddComponent(idxEntity, entity, new ProMoveComponents
                    {
                        startPosition = translation.Value,
                        endPosition = U_positions[i].Value
                    });

                    if (math.distance(position, U_positions[i].Value) < 1f)
                    {
                        entityCommandBuffer.RemoveComponent<ProMoveComponents>(idxEntity, entity);
                        entityCommandBuffer.DestroyEntity(idxEntity, entity);
                    }
                }

            }
        }

        protected override void OnCreate()
        {
            //Get all Query needed
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadWrite<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
            ProjectileQuery = GetEntityQuery(ComponentType.ReadOnly<ProjectilesComponents>(), ComponentType.ReadWrite<Translation>());
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> AllUnitPos = UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            //Creat Porjectile once we are in the fireRange (can'use as a job cause in Job we can't SpawEntity)

            //  Works with only one Unit in the scene
            for (int i = 0; i < AllUnitPos.Length; i++)
            {
                for (int j = 0; j < AllEnemiPos.Length; j++)
                {

                    if (math.distance(AllEnemiPos[j].Value, AllUnitPos[i].Value) < 18f)
                    {
                        currentTime += Time.DeltaTime;
                        if (currentTime > nextShootTime)
                        {
                            CustomEntity.SpawnEntityAtPosition(typeof(Projectiles), AllEnemiPos[j].Value);
                            float fireRate = 1f;

                            Debug.Log("Time " + currentTime + " ShootTime " + nextShootTime);
                            nextShootTime = currentTime + fireRate;
                        }

                    }

                }
            }


            /*
            Entities.WithAll<UnitComponent>().ForEach((Entity entity, int index , ref Translation trans) => {
                float3 position = trans.Value;
                for (int i = 0; i < AllEnemiPos.Length; i++)
                {
                    if (math.distance(AllEnemiPos[i].Value, AllUnitPos[index].Value) < 18f)
                    {
                        currentTime += Time.DeltaTime;
                        if (currentTime > nextShootTime)
                        {
                            CustomEntity.SpawnEntityAtPosition(typeof(Projectiles), AllEnemiPos[i].Value);
                            float fireRate = 1f;

                            Debug.Log("Time " + currentTime + " ShootTime " + nextShootTime);
                            nextShootTime = currentTime + fireRate;

                        }
                    }
                }
            });

    */


            //Job for the Projectile to arrive to the enemie position
            var job = new FindCible()
            {
                U_positions = AllUnitPos,
                entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            };
            JobHandle dependency = job.Schedule(ProjectileQuery);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(dependency);
            dependency.Complete();

            //Dispose everything
            AllUnitPos.Dispose(dependency);
            AllEnemiPos.Dispose();
        }

    }
}