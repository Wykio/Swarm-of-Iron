using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

namespace Swarm_Of_Iron_namespace
{
    public class WorkerSelectedSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<WorkerComponent, UnitSelectedComponent>().ForEach((Entity entity) => {
                Swarm_Of_Iron.instance.houseCreationButton.SetActive(true);
                return;
            });
        }
    }
}

