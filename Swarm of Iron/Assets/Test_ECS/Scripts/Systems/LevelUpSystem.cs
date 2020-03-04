using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace testECS {
    public class LevelUpSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach((ref LevelComponent levelcomponent) =>
            {
                //Increment le level à chaque frame
                levelcomponent.level += 1.0f * Time.DeltaTime;
            });
            //throw new System.NotImplementedException();
        }
    }
}