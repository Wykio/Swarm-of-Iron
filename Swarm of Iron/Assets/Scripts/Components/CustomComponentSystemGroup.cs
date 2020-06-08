using Unity.Entities;

namespace SOI {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class MoveLogicGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class MiniMapLogicGroup : ComponentSystemGroup { }
}