using Unity.Mathematics;

public struct PathRequest {
    public readonly float3 startPoint;
    public readonly float3 endPoint;
    public readonly int startIndex;
    public readonly int endIndex;

    public PathRequest(float3 startPoint, float3 endPoint, int startIndex, int endIndex) {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.startIndex = startIndex;
        this.endIndex = endIndex;
    }
}
