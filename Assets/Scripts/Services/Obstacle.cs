using Unity.Burst;

[BurstCompile]
public struct Obstacle {
    public readonly string Id;
    public readonly float Probability;

    public Obstacle(string id, float probability) {
        Id = id;
        Probability = probability;
    }
}
