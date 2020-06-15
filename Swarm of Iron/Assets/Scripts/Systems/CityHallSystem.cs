using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace SOI {

    [UpdateAfter(typeof(UnitControlSystem))]
    public class CityHallSystem : SystemBase {
        protected override void OnUpdate() {
            double time = UnityEngine.Time.time;
            int constructTime = SwarmOfIron.Instance.CityHallConstructionTime;

            Entities.ForEach((ref CityHallComponent cityHallComponent, ref NonUniformScale nonUniformScale) =>
            {
                if (time - cityHallComponent.LastConstructionStateTime >= 0.1 &&
                cityHallComponent.ConstructionState < cityHallComponent.ConstructionTime)
                {
                    nonUniformScale.Value.y += (0.05f /cityHallComponent.ConstructionTime);
                    cityHallComponent.LastConstructionStateTime = time;
                    cityHallComponent.ConstructionState++;
                }
            }).ScheduleParallel();
        }
    }
}