using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace SOI
{
    public struct RectComponent : IComponentData
    {
        public int x, y, width, height;
    }
}