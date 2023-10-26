# Project Summary
I have chosen to implement Bidirectional A* Pathfinding using Unity Jobs. This will result in very fast path calculations, even for many agents. Path Smoothing and Graph Scanning are also implemented with Unity Jobs, allowing optional path smoothing as well as quickly scanning the graph.


# Architecture
Seeker
  |
  | uses
  v
PathfindingService <----- Graph
  |                        ^
  | uses                   | schedules
  v                        |
PathRequest ----> PathfindingJob
  ^                        |
  |                        | uses
  | contains               v
  |------------------- GraphNode
  |
  | contains
  v
PathResult <---- Path


# Classes
Seeker: Game objects that are seeking a path through the game world.

Graph: Represents the game world as a 2D grid on the XZ plane and is used by the PathfindingJob to find paths.

GraphFactory: Uses RaycastCommand to scan the game world for walkable nodes

GraphTransformJob: A Unity Job that transforms the GraphFactory's RaycastCommand to a Graph

GraphNode: This class represents a node in the graph. It should contain information about its position in the world, its connections to neighboring nodes, and its cost (which could be higher for nodes that contain obstacles).

PathfindingService: An interface that provides methods for Seekers to request paths, using the JobScheduler to run Jobs and returning the resulting paths.

Path: This class represents a path through the graph as an array of world positions and the total cost of the path

PathRequest: This class represents a pathfinding request. It contains the start and end points of the requested path

PathResult: This class represents the result of a pathfinding request. It contains the resulting Path and any other relevant information like whether the pathfinding was successful.

