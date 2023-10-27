using Unity.Mathematics;

public class WorldConfig {
    public readonly float MinRadius = 3, MaxRadius = 12;

    public readonly Obstacle[] Obstacles = {
        new("Tree-1", 2),
        new("Tree-2", 9),
        new("Rock", 4)
    };

    public readonly int2 Size = new(100, 100);
}
