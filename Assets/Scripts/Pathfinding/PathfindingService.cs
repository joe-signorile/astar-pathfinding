using System.Diagnostics;
using System.Threading.Tasks;
using TinyIoC;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathfindingService {
    private readonly PathResult failedPath = new();
    private readonly GraphFactory graphFactory;
    private readonly UiService uiService;
    private Graph graph;

    public PathfindingService() {
        graphFactory = new GraphFactory();
        uiService = TinyIoCContainer.Current.Resolve<UiService>();
    }

    public async Task Start() {
        graph?.Dispose();
        graph = await graphFactory.NewGraphFromWorld();
    }

    public async Task<PathResult> FindPath(Vector3 start, Vector3 end) {
        uiService.Report(UiSupportedViews.Game, "Path Requested");
        var stopwatch = Stopwatch.StartNew();
        var pathRequest = new PathRequest(start, end, graph.GetNodeIndex(start), graph.GetNodeIndex(end));

        var rawPath = await PathfindingJobAsync(pathRequest);
        var simplePath = await PathSimplificationJobAsync(rawPath, pathRequest);
        var smoothPath = await PathSmoothJobAsync(simplePath, pathRequest);

        var pathResult = failedPath;
        var report = "Path Result: ";
        if (smoothPath.Length > 0) {
            report += "Smooth Path Found";
            pathResult = new PathResult(smoothPath);
        }
        else if (simplePath.Length > 0) {
            report += "Simple Path Found";
            pathResult = new PathResult(simplePath);
        }
        else if (rawPath.Length > 0) {
            report += "Raw Path Found";
            pathResult = new PathResult(graph, rawPath);
        }
        else
            report += "Path Failed";

        stopwatch.Stop();
        report += $"({stopwatch.ElapsedMilliseconds}ms)";

        if (rawPath.IsCreated) rawPath.Dispose();
        if (simplePath.IsCreated) simplePath.Dispose();
        if (smoothPath.IsCreated) smoothPath.Dispose();

        uiService.Report(UiSupportedViews.Game, report);

        return pathResult;
    }

    private async Task<NativeList<int>> PathfindingJobAsync(PathRequest pathRequest) {
        var delay = Task.Yield();

        var pathfindingJob = new PathfindingJob {
            pathRequest = pathRequest,
            nodes = graph.Nodes,
            path = new NativeList<int>(Allocator.Persistent)
        };

        var pathfindingJobHandle = pathfindingJob.Schedule();
        while (!pathfindingJobHandle.IsCompleted)
            await delay;

        pathfindingJobHandle.Complete();
        await delay;

        return pathfindingJob.path;
    }

    private async Task<NativeList<float3>> PathSimplificationJobAsync(NativeList<int> rawPath, PathRequest pathRequest) {
        if (rawPath.Length < 3) {
            Debug.LogWarning("Short Path");
            return new NativeList<float3>(Allocator.Persistent) { pathRequest.startPoint, pathRequest.endPoint };
        }

        var delay = Task.Yield();
        var originalPath = new NativeArray<float3>(rawPath.Length, Allocator.Persistent);
        originalPath[0] = pathRequest.startPoint;
        originalPath[originalPath.Length - 1] = pathRequest.endPoint;

        for (var i = 1; i < originalPath.Length - 1; i++) originalPath[i] = graph.Nodes[rawPath[i]].Position;

        var tolerance = graph.NodeSize * 0.75f;
        var pathSimplificationJob = new PathSimplificationJob {
            path = originalPath,
            tolerance = tolerance,
            result = new NativeList<float3>(Allocator.Persistent)
        };

        var pathSimplificationJobHandle = pathSimplificationJob.Schedule();
        while (!pathSimplificationJobHandle.IsCompleted)
            await delay;

        pathSimplificationJobHandle.Complete();
        await delay;

        return pathSimplificationJob.result;
    }

    private async Task<NativeList<float3>> PathSmoothJobAsync(NativeList<float3> simplePath, PathRequest pathRequest) {
        if (simplePath.Length < 3)
            return new NativeList<float3>(Allocator.Persistent);

        var delay = Task.Yield();

        var pathSmoothingJob = new PathSmoothingJob {
            path = simplePath,
            segments = 8,
            result = new NativeList<float3>(Allocator.Persistent)
        };

        var pathSmoothingJobHandle = pathSmoothingJob.Schedule();
        while (!pathSmoothingJobHandle.IsCompleted)
            await delay;

        pathSmoothingJobHandle.Complete();
        await delay;

        var result = pathSmoothingJob.result;
        if (result.Length > 1) {
            result[0] = pathRequest.startPoint;
            result[^1] = pathRequest.endPoint;
        }

        return result;
    }
}
