using Unity.VisualScripting;
using UnityEngine;

public class PlayerService {
    public Transform playerTransform;
    private Seeker seeker;

    public void Start() {
        var playerPrefab = Resources.Load<GameObject>("Player");
        playerTransform = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).transform;
        playerTransform.name = "Player";

        seeker = playerTransform.GetComponent<Seeker>();
        if (!seeker)
            seeker = playerTransform.AddComponent<Seeker>();
    }

    public void MoveTo(Vector3 target) {
        Stop();
        seeker.MoveTo(target);
    }

    public void Stop() {
        seeker.Stop();
    }
}
