using UnityEngine;

public class NeighborDebug : MonoBehaviour {
    [SerializeField] private int nodeIndex;
    [SerializeField] private NeighborCosts costs;
    private GizmoDrawer gizmoDrawer;
    private Graph graph;

    private void OnDrawGizmosSelected() {
        if (gizmoDrawer == null)
            return;

        gizmoDrawer.Clear();

        nodeIndex = graph.GetNodeIndex(transform.position);
        var node = graph.Nodes[nodeIndex];
        costs = node.neighborCosts;
        gizmoDrawer.Add(gizmoDrawer.Blue, node.Position, 0.5f);

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

        foreach (var neighborIndex in neighborIndices) {
            if (neighborIndex < 0 || neighborIndex >= graph.Nodes.Length) continue;

            var neighbor = graph.Nodes[neighborIndex];
            var color = neighbor.IsWalkable ? gizmoDrawer.Green : gizmoDrawer.Red;

            gizmoDrawer.Add(color, neighbor.Position, 0.5f);
        }
    }

    public void Initialize(Graph graph) {
        this.graph = graph;
        gizmoDrawer = gameObject.AddComponent<GizmoDrawer>();
    }
}
