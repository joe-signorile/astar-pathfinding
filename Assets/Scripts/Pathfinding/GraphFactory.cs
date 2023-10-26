using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class GraphFactory {
    private readonly GraphScanner graphScanner;
    private readonly GraphTransformer graphTransformer;

    public GraphFactory() {
        graphScanner = new GraphScanner();
        graphTransformer = new GraphTransformer();
    }

    public async Task<Graph> NewGraphFromWorld() {
        var graphDimensions = new int2(100, 100);
        var nodeSize = 0.5f;

        var graph = new Graph(graphDimensions, nodeSize);

        await Task.Yield();

        graphScanner.SceneCast(graph);
        await graphTransformer.Apply(graph);

        var neighborDebug = new GameObject("Neighbor Debug Tool").AddComponent<NeighborDebug>();
        neighborDebug.Initialize(graph);

        return graph;
    }
}
