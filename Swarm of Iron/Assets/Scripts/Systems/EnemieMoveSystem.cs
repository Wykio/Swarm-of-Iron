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
    public class EnemieMoveSystem : ComponentSystem
    {
        private EntityQuery EnemiQuery, UnitQuery;

        [BurstCompile]
        struct FindTarget : IJobParallelFor
        {
            [NativeDisableParallelForRestriction]  public NativeArray<Translation> U_positions;
            [ReadOnly] public NativeArray<Translation> E_positions;
            //float3 destination;


            public void Execute(int index)
            {


                //float3 position = U_positions[index].Value;
                //float3 Epositions = E_positions[index].Value;
                //bool Targetfound = false;
                    
                    for (int i = 0; i < E_positions.Length; i++)
                    {
                        if (Vector3.Distance(U_positions[index].Value, E_positions[i].Value) < 30f)
                        {
                            float dist = Vector3.Distance(U_positions[index].Value, E_positions[i].Value);
                            float3 p = E_positions[i].Value;
                            float3 t = U_positions[index].Value;

                            Debug.Log("Distance111111: " + dist + " EPos: " + E_positions[i].Value + " UPos: " + U_positions[index].Value);
                            U_positions[index] = new Translation { Value = E_positions[i].Value } ;
                            Debug.Log("Distance: " + dist + " EPos: " + E_positions[i].Value + " UPos: " + U_positions[index].Value);
                        }
                    }
                    
            }
            

        }

        public static JobHandle Schedule(NativeArray<Translation> Uposition, NativeArray<Translation> Eposition)
        {
            var job = new FindTarget()
            {
                U_positions = Uposition,
                E_positions = Eposition,
                
            };
            return job.Schedule(Uposition.Length, 1);
        }



        protected override void OnCreate()
        {
            EnemiQuery = GetEntityQuery(ComponentType.ReadOnly<E_UnitComponent>(), ComponentType.ReadOnly<Translation>());
            UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override void OnUpdate() 
        {
            NativeArray<Translation> AllEnemiPos = EnemiQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> AllUnitPos = UnitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);


            JobHandle jobHandle = Schedule(AllUnitPos, AllEnemiPos);
            jobHandle.Complete();


            AllEnemiPos.Dispose();
            AllUnitPos.Dispose();

        }



    }

} 