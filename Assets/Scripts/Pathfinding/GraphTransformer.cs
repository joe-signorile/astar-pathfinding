using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class GraphTransformer {
    public async Task Apply(Graph graph) {
        var nodes = graph.Nodes;
        await Task.Run(() => {
            for (var i = 0; i < nodes.Length; i++) {
                var node = graph.Nodes[i];
                var x = i / graph.GraphSize.y;
                var z = i % graph.GraphSize.y;

                node.neighbors.north = z > 0 ? i - 1 : -1;
                node.neighbors.south = z < graph.GraphSize.y - 1 ? i + 1 : -1;
                node.neighbors.west = x > 0 ? i - graph.GraphSize.y : -1;
                node.neighbors.east = x < graph.GraphSize.x - 1 ? i + graph.GraphSize.y : -1;
                node.neighbors.northeast = node.neighbors.north != -1 && node.neighbors.east != -1 ? node.neighbors.north + graph.GraphSize.y : -1;
                node.neighbors.northwest = node.neighbors.north != -1 && node.neighbors.west != -1 ? node.neighbors.north - graph.GraphSize.y : -1;
                node.neighbors.southeast = node.neighbors.south != -1 && node.neighbors.east != -1 ? node.neighbors.south + graph.GraphSize.y : -1;
                node.neighbors.southwest = node.neighbors.south != -1 && node.neighbors.west != -1 ? node.neighbors.south - graph.GraphSize.y : -1;

                var neighborIndices = new[] {
                    node.neighbors.north,
                    node.neighbors.south,
                    node.neighbors.east,
                    node.neighbors.west,
                    node.neighbors.northeast,
                    node.neighbors.northwest,
                    node.neighbors.southeast,
                    node.neighbors.southwest
                };

                var neighborDistances = new float[8];

                for (var j = 0; j < neighborIndices.Length; j++) {
                    var neighborIndex = neighborIndices[j];

                    if (neighborIndex < 0 || neighborIndex >= nodes.Length || !nodes[neighborIndex].IsWalkable) {
                        neighborDistances[j] = float.PositiveInfinity;
                        continue;
                    }

                    neighborDistances[j] = Vector3.Distance(nodes[neighborIndex].Position, nodes[i].Position);

                    var unwalkableNeighborCount = 0;
                    var neighborNode = nodes[neighborIndex];
                    var neighborNeighborIndices = new[] {
                        neighborNode.neighbors.north,
                        neighborNode.neighbors.south,
                        neighborNode.neighbors.east,
                        neighborNode.neighbors.west,
                        neighborNode.neighbors.northeast,
                        neighborNode.neighbors.northwest,
                        neighborNode.neighbors.southeast,
                        neighborNode.neighbors.southwest
                    };

                    for (var k = 0; k < neighborNeighborIndices.Length; k++) {
                        var neighborNeighborIndex = neighborNeighborIndices[k];
                        if (neighborNeighborIndex < 0 || neighborNeighborIndex >= nodes.Length || !nodes[neighborNeighborIndex].IsWalkable) unwalkableNeighborCount++;
                    }

                    if (unwalkableNeighborCount > 0) {
                        var costMultiplier = 1 + 0.5f * unwalkableNeighborCount;
                        neighborDistances[j] *= costMultiplier;
                    }
                }

                node.neighborCosts.north = neighborDistances[0];
                node.neighborCosts.south = neighborDistances[1];
                node.neighborCosts.east = neighborDistances[2];
                node.neighborCosts.west = neighborDistances[3];
                node.neighborCosts.northeast = neighborDistances[4];
                node.neighborCosts.northwest = neighborDistances[5];
                node.neighborCosts.southeast = neighborDistances[6];
                node.neighborCosts.southwest = neighborDistances[7];

                graph.Nodes[i] = node;
            }
        });
    }
}
