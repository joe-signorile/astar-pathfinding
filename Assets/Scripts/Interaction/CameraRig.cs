using UnityEngine;

public class CameraRig : MonoBehaviour {
    [SerializeField] private float maxSpeed = 50, damping = 100;
    private Transform target;
    private Vector3 velocity;

    private void Update() {
        if (!target)
            return;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        var n1 = velocity - (transform.position - target.position) * (damping * damping * Time.deltaTime);
        var n2 = 1 + damping * Time.deltaTime;
        velocity = n1 / (n2 * n2);

        transform.position += velocity * Time.deltaTime;
    }

    public void SetTarget(Transform target) {
        velocity = Vector3.zero;
        this.target = target;
    }
}
