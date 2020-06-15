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
            [ReadOnly] public NativeArray<Translation> U_positions;
            //public NativeArray<Translation> E_positions;
            

            public void Execute(Entity entity, int idxEntity, ref Translation translation)
            {
                float3 position = translation.Value;

                for (int i = 0; i < U_positions.Length; i++)
                {
                    if (math.distance(position, U_positions[i].Value) < 5f)
                    {
                        translation.Value = U_positions[i].Value+1;
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadWrite<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override void OnUpdate()
        {
           // NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> AllUnitPos = UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            var job = new FindTarget()
            {
                U_positions = AllUnitPos,
                //E_positions = AllEnemiPos
            };
            JobHandle dependency = job.Schedule(EnemiQuery);

           // AllEnemiPos.Dispose(dependency);
            AllUnitPos.Dispose(dependency);
        }
    }
}