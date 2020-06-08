using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;

namespace SOI {
    public class Flowfield {

        public static void Construct(NativeArray<int2> flowfield, in NativeArray<int> dijkstraGrid, in int2 position, in int2 target, in int _width, in int _height, in int _max) {

            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    int index = x + y * _width;

                    if (dijkstraGrid[index] != _max) {
                        int2 pos = new int2(x, y);

                        NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);
                        allNeighboursOf(pos, 0, 0, _width, neighbours);

                        int2 min = new int2(0, 0);
                        float minDist = _max;
                        for (var j = 0; j < neighbours.Length; j++) {
                            int2 n = neighbours[j];
                            float dist = dijkstraGrid[n[0] + (n[1] * _width)] - dijkstraGrid[index];

                            if (dist < minDist) {
                                min = n;
                                minDist = dist;
                            }
                        }

                        if (minDist < _max) flowfield[index] = new int2(math.normalize(min - pos));
                        neighbours.Dispose();
                    }
                }
            }

            flowfield[target[0] + (target[1] * _width)] = new int2(0, 0);
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
