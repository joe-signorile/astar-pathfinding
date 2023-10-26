using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct PathResult {
    public Queue<Vector3> Path;

    public PathResult(NativeList<float3> pathResult) {
        Path = new Queue<Vector3>(pathResult.Length);
        foreach (var point in pathResult)
            Path.Enqueue(point);
    }

    public PathResult(Graph graph, NativeList<int> pathResult) {
        Path = new Queue<Vector3>(pathResult.Length);
        foreach (var index in pathResult)
            Path.Enqueue(graph.Nodes[index].Position);
    }

    public Vector3 Dequeue() {
        return Path.Dequeue();
    }
}
