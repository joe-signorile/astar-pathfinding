using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Graph {
    public readonly int2 GraphSize;
    public readonly int NodeCount;
    public readonly float NodeSize;
    public NativeArray<GraphNode> Nodes;

    public Graph(int2 graphDimensions, float nodeSize) {
        NodeSize = nodeSize;
        GraphSize = new int2((int)(graphDimensions.x / NodeSize), (int)(graphDimensions.y / NodeSize));
        NodeCount = GraphSize.x * GraphSize.y;
        Nodes = new NativeArray<GraphNode>(NodeCount, Allocator.Persistent);

        for (var x = 0; x < GraphSize.x; x++) {
            for (var z = 0; z < GraphSize.y; z++) {
                var nodeIndex = x * GraphSize.y + z;
                var position = new float3(x * NodeSize - GraphSize.x * NodeSize / 2.0f, 0, z * NodeSize - GraphSize.y * NodeSize / 2.0f);
                Nodes[nodeIndex] = new GraphNode(nodeIndex, position);
            }
        }
    }

    public int GetNodeIndex(Vector3 point) {
        var x = Mathf.Clamp((int)((point.x + GraphSize.x * NodeSize / 2.0f) / NodeSize), 0, GraphSize.x - 1);
        var z = Mathf.Clamp((int)((point.z + GraphSize.y * NodeSize / 2.0f) / NodeSize), 0, GraphSize.y - 1);
        return x * GraphSize.y + z;
    }

    public void Dispose() {
        Nodes.Dispose();
    }
}
