using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public struct RectComponent : IComponentData
    {
        public int x, y, width, height;
    }
}