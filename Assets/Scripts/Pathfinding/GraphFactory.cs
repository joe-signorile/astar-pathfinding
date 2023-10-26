using System.Diagnostics;
using System.Threading.Tasks;
using TinyIoC;
using Unity.Mathematics;
using UnityEngine;

public class GraphFactory {
    private readonly GraphScanner graphScanner;
    private readonly GraphTransformer graphTransformer;
    private readonly UiService uiService;

    public GraphFactory() {
        graphScanner = new GraphScanner();
        graphTransformer = new GraphTransformer();
        uiService = TinyIoCContainer.Current.Resolve<UiService>();
    }

    public async Task<Graph> NewGraphFromWorld() {
        var stopwatch = Stopwatch.StartNew();
        var graphDimensions = new int2(100, 100);
        var nodeSize = 0.5f;

        var graph = new Graph(graphDimensions, nodeSize);

        await Task.Yield();

        graphScanner.SceneCast(graph);
        await graphTransformer.Apply(graph);

        var neighborDebug = new GameObject("Neighbor Debug Tool").AddComponent<NeighborDebug>();
        neighborDebug.Initialize(graph);

        stopwatch.Stop();
        var report = $"Graph Scan: {stopwatch.ElapsedMilliseconds}ms ({graph.NodeCount} Nodes)";
        uiService.Report(UiSupportedViews.World, report);

        return graph;
    }
}
