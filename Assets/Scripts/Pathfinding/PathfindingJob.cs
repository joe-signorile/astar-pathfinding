using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct PathfindingJob : IJob {
    [ReadOnly] public PathRequest pathRequest;
    [ReadOnly] public NativeArray<GraphNode> nodes;
    public NativeList<int> path;

    public void Execute() {
        var startNodeIndex = pathRequest.startIndex;
        var endNodeIndex = pathRequest.endIndex;

        if (!nodes[startNodeIndex].IsWalkable || !nodes[endNodeIndex].IsWalkable)
            throw new Exception("Unwalkable start or end node");

        var openForward = new NativeList<int>(Allocator.Temp);
        var openBackward = new NativeList<int>(Allocator.Temp);
        var gForward = new NativeArray<float>(nodes.Length, Allocator.Temp);
        var gBackward = new NativeArray<float>(nodes.Length, Allocator.Temp);
        var parentForward = new NativeArray<int>(nodes.Length, Allocator.Temp);
        var parentBackward = new NativeArray<int>(nodes.Length, Allocator.Temp);

        for (var i = 0; i < nodes.Length; i++) {
            gForward[i] = float.PositiveInfinity;
            gBackward[i] = float.PositiveInfinity;
            parentForward[i] = -1;
            parentBackward[i] = -1;
        }

        openForward.Add(startNodeIndex);
        openBackward.Add(endNodeIndex);

        gForward[startNodeIndex] = 0;
        gBackward[endNodeIndex] = 0;

        while (openForward.Length > 0 && openBackward.Length > 0) {
            var currentForward = GetLowestCostNode(openForward, gForward);
            var currentBackward = GetLowestCostNode(openBackward, gBackward);

            var pathFound = false;
            for (var i = 0; i < nodes.Length; i++)
                if (parentForward[i] != -1 && parentBackward[i] != -1) {
                    currentForward = i;
                    currentBackward = i;
                    pathFound = true;
                    break;
                }

            if (pathFound) {
                ConstructPath(currentForward, currentBackward, parentForward, parentBackward);
                break;
            }

            ExpandNode(currentForward, openForward, gForward, parentForward);
            ExpandNode(currentBackward, openBackward, gBackward, parentBackward);
        }

        openForward.Dispose();
        openBackward.Dispose();
        gForward.Dispose();
        gBackward.Dispose();
        parentForward.Dispose();
        parentBackward.Dispose();
    }

    private int GetLowestCostNode(NativeList<int> openList, NativeArray<float> fValues) {
        var lowestCostNode = openList[0];
        for (var i = 1; i < openList.Length; i++)
            if (fValues[openList[i]] < fValues[lowestCostNode])
                lowestCostNode = openList[i];
        return lowestCostNode;
    }

    private void ExpandNode(int nodeIndex, NativeList<int> openList, NativeArray<float> gValues, NativeArray<int> parents) {
        var currentNode = nodes[nodeIndex];
        if (!currentNode.IsWalkable) return;

        var neighbors = currentNode.neighbors;
        var neighborCosts = currentNode.neighborCosts;

        var neighborIndices = new NativeArray<int>(8, Allocator.Temp);
        neighborIndices[0] = neighbors.north;
        neighborIndices[1] = neighbors.south;
        neighborIndices[2] = neighbors.east;
        neighborIndices[3] = neighbors.west;
        neighborIndices[4] = neighbors.northeast;
        neighborIndices[5] = neighbors.northwest;
        neighborIndices[6] = neighbors.southeast;
        neighborIndices[7] = neighbors.southwest;

        var neighborDistances = new NativeArray<float>(8, Allocator.Temp);
        neighborDistances[0] = neighborCosts.north;
        neighborDistances[1] = neighborCosts.south;
        neighborDistances[2] = neighborCosts.east;
        neighborDistances[3] = neighborCosts.west;
        neighborDistances[4] = neighborCosts.northeast;
        neighborDistances[5] = neighborCosts.northwest;
        neighborDistances[6] = neighborCosts.southeast;
        neighborDistances[7] = neighborCosts.southwest;

        for (var i = 0; i < neighborIndices.Length; i++) {
            var neighborIndex = neighborIndices[i];
            if (neighborIndex < 0 || neighborIndex >= nodes.Length || !nodes[neighborIndex].IsWalkable) continue;

            var tentativeG = gValues[nodeIndex] + neighborDistances[i];

            if (tentativeG < gValues[neighborIndex]) {
                parents[neighborIndex] = nodeIndex;
                gValues[neighborIndex] = tentativeG;

                if (!openList.Contains(neighborIndex)) openList.Add(neighborIndex);
            }
        }

        if (openList.Contains(nodeIndex)) RemoveByValue(openList, nodeIndex);

        neighborIndices.Dispose();
        neighborDistances.Dispose();
    }

    private void ConstructPath(int meetingPointForward, int meetingPointBackward, NativeArray<int> parentForward, NativeArray<int> parentBackward) {
        var currentNodeIndex = meetingPointForward;
        while (currentNodeIndex != -1 && currentNodeIndex != pathRequest.startIndex) {
            path.Add(currentNodeIndex);
            currentNodeIndex = parentBackward[currentNodeIndex];
        }

        ReverseList(path);

        if (meetingPointForward != meetingPointBackward) path.Add(meetingPointBackward);

        currentNodeIndex = parentForward[meetingPointBackward];
        while (currentNodeIndex != -1 && currentNodeIndex != pathRequest.endIndex) {
            path.Add(currentNodeIndex);
            currentNodeIndex = parentForward[currentNodeIndex];
        }

        if (currentNodeIndex == pathRequest.endIndex) path.Add(pathRequest.endIndex);

        ReverseList(path);
    }


    private void ReverseList(NativeList<int> list) {
        var length = list.Length;
        var halfLength = length / 2;
        for (var i = 0; i < halfLength; i++)
            (list[i], list[length - i - 1]) = (list[length - i - 1], list[i]);
    }

    private void RemoveByValue(NativeList<int> list, int value) {
        for (var i = 0; i < list.Length; i++)
            if (list[i] == value) {
                list.RemoveAt(i);
                break;
            }
    }
}
