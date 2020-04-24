using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Swarm_Of_Iron_namespace
{
    public class CityHallSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            double time = UnityEngine.Time.time;
            int constructTime = Swarm_Of_Iron.instance.CityHallConstructionTime;

            return Entities.ForEach((ref CityHallComponent cityHallComponent, ref NonUniformScale nonUniformScale) =>
            {
                if (time - cityHallComponent.LastConstructionStateTime >= 2 &&
                cityHallComponent.ConstructionState < cityHallComponent.ConstructionTime)
                {
                    nonUniformScale.Value.y += (0.05f /cityHallComponent.ConstructionTime);
                    cityHallComponent.LastConstructionStateTime = time;
                    cityHallComponent.ConstructionState++;
                }
            }).Schedule(inputDeps);
        }
    }
}