using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct PathSmoothingJob : IJob {
    [ReadOnly] public NativeList<float3> path;
    public NativeList<float3> result;
    public int segments;

    public void Execute() {
        if (path.Length < 4)
            return;

        for (var i = 0; i < path.Length - 3; i++) {
            var p0 = path[i];
            var p1 = path[i + 1];
            var p2 = path[i + 2];
            var p3 = path[i + 3];

            for (var j = 0; j < segments; j++) {
                var t = (float)j / segments;

                var point = CatmullRomSpline(p0, p1, p2, p3, t);
                result.Add(point);
            }
        }

        if (path.Length % 4 != 0) {
            var lastIndex = path.Length - 1;
            var lastAddedIndex = result.Length - 1;
            var distance = math.distance(path[lastIndex], result[lastAddedIndex]);

            if (distance > 0.01f)
                for (var k = 1; k <= segments; k++) {
                    var t = (float)k / segments;
                    var point = math.lerp(result[lastAddedIndex], path[lastIndex], t);

                    result.Add(point);
                }
        }
    }

    private float3 CatmullRomSpline(float3 p0, float3 p1, float3 p2, float3 p3, float t) {
        var t2 = t * t;
        var t3 = t2 * t;
        var a = 0.5f * (2f * p1);
        var b = 0.5f * (p2 - p0);
        var c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        var d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);
        return a + b * t + c * t2 + d * t3;
    }
}
