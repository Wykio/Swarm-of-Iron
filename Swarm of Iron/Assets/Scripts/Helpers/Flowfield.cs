using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;

namespace SOI {
    public class Flowfield {

        public static void Construct(NativeArray<int2> flowField, NativeArray<int> dijkstraGrid, int2 position, int2 target, int _width, int _height, int _max) {
            int marg = 2;
            int padd = 1;
            int baseX = (int)math.floor(position[0] / marg) * marg;
            int baseY = (int)math.floor(position[1] / marg) * marg;

            for (var x = baseX - padd; x < baseX + marg + padd; x++) {
                for (var y = baseY - padd; y < baseY + marg + padd; y++) {
                    int index = x + y * _width;

                    if (dijkstraGrid[index] != _max) {
                        int2 pos = new int2(x, y);

                        NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);
                        allNeighboursOf(pos, baseX - padd, baseY - padd, marg + (padd * 2), neighbours);

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

                        if (minDist < _max) flowField[index] = new int2(math.normalize(min - pos));
                        neighbours.Dispose();
                    }
                }
            }

            flowField[target[0] + (target[1] * _width)] = new int2(0, 0);
        }

        private static void allNeighboursOf(int2 pos, int top, int left, int size, NativeArray<int2> res)
        {
            var index = 0;

            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = pos[0] + dx;
                    var y = pos[1] + dy;

                    //All neighbours on the grid that aren't ourself
                    if (x >= top && y >= left && x < top + size && y < left + size && !(dx == 0 && dy == 0))
                    {
                        res[index++] = new int2(x, y);
                    }
                }
            }

            res = res.GetSubArray(0, index);
        }
    }
}
