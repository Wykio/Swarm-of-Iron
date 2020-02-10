using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// components are always struct
public struct LevelComponent : IComponentData
{
    public float level;
}
