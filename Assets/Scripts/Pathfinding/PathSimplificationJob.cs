using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct PathSimplificationJob : IJob {
    public NativeArray<float3> path;
    public NativeList<float3> result;
    public float tolerance, NodeSize;
    public NativeArray<GraphNode> graphNodes; // Pass your GraphNodes array to the job.

    public void Execute() {
        // Ramer-Douglas-Peucker algorithm
        var maxDistance = 0f;
        var maxIndex = -1;
        var firstPoint = path[0];
        var lastPoint = path[path.Length - 1];

        for (var i = 1; i < path.Length - 1; i++) {
            var point = path[i];

            // Check if the line segment intersects with unwalkable nodes.
            if (!LineIntersectsUnwalkable(firstPoint, point)) {
                var distance = DistanceToLine(point, firstPoint, lastPoint);

                if (distance > maxDistance) {
                    maxDistance = distance;
                    maxIndex = i;
                }
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

            // Check if the line segment intersects with unwalkable nodes.
            if (!LineIntersectsUnwalkable(startPoint, point)) {
                var distance = DistanceToLine(point, startPoint, endPoint);

                if (distance > maxDistance) {
                    maxDistance = distance;
                    maxIndex = i;
                }
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

    private bool LineIntersectsUnwalkable(float3 start, float3 end) {
        for (var i = 0; i < graphNodes.Length; i++) {
            var node = graphNodes[i];

            if (!node.IsWalkable) {
                var nodePosition = node.Position;

                // Check if the line segment intersects with the bounds of the unwalkable node.
                if (LineIntersectsBounds(start, end, nodePosition)) return true;
            }
        }

        return false;
    }

    private bool LineIntersectsBounds(float3 start, float3 end, float3 nodePosition) {
        // Check if the line segment intersects with the bounding box of the unwalkable node.
        // You should adapt this logic based on your specific definition of a node's bounds.
        // This example assumes that the node's bounds are a cube centered around nodePosition.

        // Define the half-size of the node's bounds (assuming it's a cube).
        var halfNodeSize = NodeSize / 2.0f; // You may need to adapt this based on your node's size.

        // Calculate the min and max bounds of the node.
        var minBounds = nodePosition - halfNodeSize;
        var maxBounds = nodePosition + halfNodeSize;

        // Check if the line segment intersects with the node's bounds.
        return start.x <= maxBounds.x && end.x >= minBounds.x &&
               start.y <= maxBounds.y && end.y >= minBounds.y &&
               start.z <= maxBounds.z && end.z >= minBounds.z;
    }
}
