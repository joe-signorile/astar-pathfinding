using UnityEngine;

public class Masks {
    public readonly LayerMask Ground;
    public readonly LayerMask Raycast;

    public Masks() {
        Ground = LayerMask.GetMask("Ground");
        Raycast = LayerMask.GetMask("Ground", "Obstacle");
    }
}
