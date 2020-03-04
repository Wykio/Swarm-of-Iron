using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace testECS {
    public class MoverSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, ref MoveSpeedComponent moveSpeedComponent) =>
            {
                //Increment la translation à chaque frame
                translation.Value.y += moveSpeedComponent.moveSpeed * Time.DeltaTime;
                if (translation.Value.y > 5.0f)
                {
                    moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
                }
                if (translation.Value.y < -5.0f)
                {
                    moveSpeedComponent.moveSpeed = +math.abs(moveSpeedComponent.moveSpeed);
                }
            });
            //throw new System.NotImplementedException();
        }
    }
}