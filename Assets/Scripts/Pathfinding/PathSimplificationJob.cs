using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct PathSimplificationJob : IJob {
    public NativeArray<float3> path;
    public NativeList<float3> result;
    public float tolerance;

    public void Execute() {
        // Ramer-Douglas-Peucker algorithm
        var maxDistance = 0f;
        var maxIndex = -1;
        var firstPoint = path[0];
        var lastPoint = path[path.Length - 1];

        for (var i = 1; i < path.Length - 1; i++) {
            var point = path[i];
            var distance = DistanceToLine(point, firstPoint, lastPoint);

            if (distance > maxDistance) {
                maxDistance = distance;
                maxIndex = i;
            }
        }

        if (maxDistance > tolerance) {
            Simplify(0, maxIndex, result, tolerance);
            Simplify(maxIndex, path.Length - 1, result, tolerance);
        }
        else {
            result.Add(firstPoint);
            result.Add(lastPoint);
        }
    }

    private void Simplify(int start, int end, NativeList<float3> result, float tolerance) {
        var maxDistance = 0f;
        var maxIndex = -1;
        var startPoint = path[start];
        var endPoint = path[end];

        for (var i = start + 1; i < end; i++) {
            var point = path[i];
            var distance = DistanceToLine(point, startPoint, endPoint);

            if (distance > maxDistance) {
                maxDistance = distance;
                maxIndex = i;
            }
        }

        if (maxDistance > tolerance) {
            Simplify(start, maxIndex, result, tolerance);
            Simplify(maxIndex, end, result, tolerance);
        }
        else
            result.Add(endPoint);
    }

    private float DistanceToLine(float3 point, float3 lineStart, float3 lineEnd) {
        var lineLengthSquared = math.lengthsq(lineEnd - lineStart);
        if (lineLengthSquared == 0f) return math.length(point - lineStart);
        var t = math.dot(point - lineStart, lineEnd - lineStart) / lineLengthSquared;

        if (t < 0f) return math.length(point - lineStart);
        if (t > 1f) return math.length(point - lineEnd);

        var projection = lineStart + t * (lineEnd - lineStart);
        return math.length(point - projection);
    }
}
