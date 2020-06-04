using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;

namespace Swarm_Of_Iron_namespace {
    public class FlowFieldHelper {

        public static void Construct(NativeArray<float2> flowField, [ReadOnly] NativeArray<int> dijkstraGrid, [ReadOnly] int width, [ReadOnly] int height, [ReadOnly] int MAX_VALUE) {
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    int index = x + y * width;
                    flowField[index] = new float2(0, 0);

                    //Obstacles have no flow value
                    if (dijkstraGrid[index] != MAX_VALUE) {
                        int2 pos = new int2(x, y);

                        NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);
                        allNeighboursOf(pos, 0, 0, width, neighbours);

                        //Go through all neighbours and find the one with the lowest distance
                        int2 min = new int2(0, 0);
                        float minDist = MAX_VALUE;
                        for (var j = 0; j < neighbours.Length; j++) {
                            int2 n = neighbours[j];
                            float dist = dijkstraGrid[n[0] + (n[1] * width)] - dijkstraGrid[index];

                            if (dist < minDist) {
                                min = n;
                                minDist = dist;
                            }
                        }

                        //If we found a valid neighbour, point in its direction
                        if (minDist < MAX_VALUE) {
                            flowField[index] = math.normalize(min - pos);
                        }
                    
                        neighbours.Dispose();
                    }
                }
            }
        }

        public static void allNeighboursOf(int2 pos, int top, int left, int size, NativeArray<int2> res) {
            var index = 0;

            for (var dx = -1; dx <= 1; dx++) {
                for (var dy = -1; dy <= 1; dy++) {
                    var x = pos[0] + dx;
                    var y = pos[1] + dy;

                    //All neighbours on the grid that aren't ourself
                    if (x >= top && y >= left && x < top + size && y < left + size && !(dx == 0 && dy == 0)) {
                        res[index++] = new int2(x, y);
                    }
                }
            }

            res = res.GetSubArray(0, index);
        }
    }
}
