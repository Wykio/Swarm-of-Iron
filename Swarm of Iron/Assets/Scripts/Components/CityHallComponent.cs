using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace SOI
{
    public struct CityHallComponent : IComponentData
    {
        //construct speed
        public float ConstructionTime;
        //state of construct
        public int ConstructionState;
        //last ConstructionState time
        public double LastConstructionStateTime;
    }
}
