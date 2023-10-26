using Unity.Mathematics;

public struct GraphNode {
    public GraphNode(int index, float3 position) {
        Index = index;
        Position = position;
        IsWalkable = false;
        neighbors = new Neighbors();
        neighborCosts = new NeighborCosts();
    }

    public int Index { get; }
    public bool IsWalkable { get; set; }
    public float3 Position { get; private set; }
    public Neighbors neighbors;
    public NeighborCosts neighborCosts;

    public void SetHeight(float height) {
        Position = new float3(Position.x, height, Position.z);
    }
}
