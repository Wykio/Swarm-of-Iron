using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace SOI
{
    public class FarmingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.ForEach((ref Translation translation, ref MoveToComponent moveTo) =>
            {
                if (moveTo.harvest)
                {
                    //goldAmount += 0.1f;
                    SwarmOfIron.Instance.goldAmount += 0.005f;
                }
            }).Schedule(inputDeps);
        }
    }
}
