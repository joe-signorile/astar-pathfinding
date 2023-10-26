using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GraphScanner {
    private readonly Masks masks;
    private readonly float rayOffset = 0.45f;

    public GraphScanner() {
        masks = new Masks();
    }

    public void SceneCast(Graph graph) {
        var raysPerNode = 9;

        var offset = graph.NodeSize * 0.5f * rayOffset;
        float3[] offsets = {
            new(0f, 0f, 0f), // center
            new(0f, 0f, offset), // north
            new(0f, 0f, -offset), // south
            new(offset, 0f, 0f), // east
            new(-offset, 0f, 0f), // west
            new(offset, 0f, offset), // northeast
            new(-offset, 0f, offset), // northwest
            new(offset, 0f, -offset), // southeast
            new(-offset, 0f, -offset) // southwest
        };

        var raycastCommands = new NativeArray<RaycastCommand>(graph.NodeCount * raysPerNode, Allocator.TempJob);
        var raycastHits = new NativeArray<RaycastHit>(graph.NodeCount * raysPerNode, Allocator.TempJob);

        var originOffset = new float3(0, 100, 0);

        for (var i = 0; i < graph.Nodes.Length; i++) {
            var node = graph.Nodes[i];
            for (var j = 0; j < raysPerNode; j++) {
                var raycastOrigin = node.Position + originOffset + offsets[j];

                var raycastParams = new QueryParameters(masks.Raycast, hitTriggers: QueryTriggerInteraction.Ignore);
                var raycastCommand = new RaycastCommand(Physics.defaultPhysicsScene, raycastOrigin, Vector3.down, raycastParams);

                var commandIndex = i * raysPerNode + j;
                raycastCommands[commandIndex] = raycastCommand;
            }
        }

        var raycastJobHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 10);
        raycastJobHandle.Complete();

        var gizmoDrawer = new GameObject("Raycast Results").AddComponent<GizmoDrawer>();

        for (var i = 0; i < graph.Nodes.Length; i++) {
            var node = graph.Nodes[i];
            var isWalkable = true;
            var height = 0f;

            for (var j = 0; j < raysPerNode; j++) {
                var hitIndex = i * raysPerNode + j;
                var hit = raycastHits[hitIndex];
                if (hit.collider != null) {
                    var hitLayer = hit.collider.gameObject.layer;

                    if ((masks.Ground & (1 << hitLayer)) == 0)
                        isWalkable = false;
                    else if (j == 0)
                        height = hit.point.y;
                }
                else
                    isWalkable = false;
            }

            node.IsWalkable = isWalkable;
            node.SetHeight(height);
            graph.Nodes[i] = node;

            var color = isWalkable ? gizmoDrawer.Green : gizmoDrawer.Red;
            gizmoDrawer.Add(color, node.Position, 0.2f);
        }
    }
}
