using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace testECS {
    public struct MoveSpeedComponent : IComponentData
    {
        public float moveSpeed;
    }
}