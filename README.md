# Project Summary
 - Bidirectional A* Pathfinding using Unity Jobs and Burst with optional Path Smoothing.
 - Graph Scanning is done against the 3d world, with an example generator at `WorldService`.
 - Third party libraries:
    - Dependency injection`Scripts/TinyIoC.cs`
    - Procedural generation `Scripts/FastPoissonDiskSampling`


# Usage
The entrypoint is `/Scripts/AppLoader.cs`

The pathfinding solution is at `/Scripts/Pathfinding/`

`Seeker.MoveTo(..)` Calls `PathfindingService.FindPath(...)`


# Architecture
![alt class diagram](https://github.com/joe-signorile/astar-pathfinding/blob/main/classes.png)


# Classes
Seeker: Game objects that are seeking a path through the game world.

Graph: Represents the game world as a 2D grid on the XZ plane and is used by the PathfindingService to find paths.

GraphFactory: Uses RaycastCommand to scan the game world for walkable nodes.

GraphTransformJob: A Unity Job that transforms the GraphFactory's RaycastCommand to a Graph.

GraphNode: This class represents a node in the graph. It contains information about its position in the world, its connections to neighboring nodes, and its cost.

PathfindingService: An interface that provides a method for Seekers to request paths.

PathRequest: This class represents a pathfinding request. It contains the start and end points of the requested path as both node indices and world positions.

PathResult: This class represents the result of a pathfinding request. It transforms the job outputs to useable paths.

