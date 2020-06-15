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

        [BurstCompile]
        struct FindTarget : IJobForEachWithEntity<Translation>
        {
            [ReadOnly] public NativeArray<Translation> E_positions;
            public void Execute(Entity entity, int idxEntity, ref Translation translation)
            {
                float3 position = translation.Value;

                for (int i = 0; i < E_positions.Length; i++)
                {
                    if (math.distance(position, E_positions[i].Value) < 5f)
                    {
                        translation.Value = E_positions[i].Value;
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadOnly<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadWrite<Translation>());
        }

        protected override void OnUpdate()
        {
            NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new FindTarget()
            {
                E_positions = AllEnemiPos,
            };
            JobHandle dependency = job.Schedule(UnitQuery);

            AllEnemiPos.Dispose(dependency);
        }
    }
}