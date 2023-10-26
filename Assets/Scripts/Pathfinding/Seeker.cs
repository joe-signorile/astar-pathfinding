using TinyIoC;
using UnityEngine;

public class Seeker : MonoBehaviour {
    private static readonly Vector3 DebugSize = new(0.1f, 0.1f, 0.1f);
    [SerializeField] private float speed;

    private Vector3 goal;
    private PathfindingService pathfindingService;
    private PathResult pathResult;

    private void Start() {
        var container = TinyIoCContainer.Current;
        pathfindingService = container.Resolve<PathfindingService>();
        goal = transform.position;
        speed = 5;
    }

    private void LateUpdate() {
        if (goal != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
            return;
        }

        if (pathResult.Path?.Count > 0)
            goal = pathResult.Dequeue();
    }

    private void OnDrawGizmosSelected() {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.blue;
        if (pathResult.Path?.Count > 0)
            foreach (var point in pathResult.Path)
                Gizmos.DrawCube(point, DebugSize);
    }

    public async void MoveTo(Vector3 target) {
        pathResult = await pathfindingService.FindPath(transform.position, target);
    }

    public void Stop() {
        pathResult = new PathResult();
        goal = transform.position;
    }
}
