using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour {
    public readonly Color32 Blue = new(0, 0, 255, 200);
    public readonly Color32 Green = new(0, 255, 0, 200);
    public readonly Color32 Red = new(255, 0, 0, 200);
    private Dictionary<Color32, Dictionary<float3, float>> gizmos;

    public GizmoDrawer() {
        Clear();
    }

    private void OnDrawGizmosSelected() {
        foreach (var colorSet in gizmos) {
            Gizmos.color = colorSet.Key;
            foreach (var gizmoSettings in colorSet.Value) Gizmos.DrawWireSphere(gizmoSettings.Key, gizmoSettings.Value);
        }
    }

    public void Clear() {
        gizmos = new Dictionary<Color32, Dictionary<float3, float>>();
    }

    public void Add(Color32 color, float3 position, float radius) {
        if (!gizmos.ContainsKey(color))
            gizmos[color] = new Dictionary<float3, float>();

        gizmos[color].Add(position, radius);
    }
}
