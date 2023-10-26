using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WorldService {
    private readonly string[] obstacleNames = { "Tree-1", "Tree-2", "Rock" };
    private float2 center;
    private float minRadius, maxRadius;
    private GameObject[] obstaclePrefabs;
    private float[] obstacleProbabilities;
    private Transform worldContainer;
    public int2 Size { get; private set; }

    public async Task Start() {
        Size = new int2(100, 100);
        center = new float2(Size.x * 0.5f, Size.y * 0.5f);
        minRadius = 3;
        maxRadius = 15;
        var delay = Task.Yield();

        worldContainer = GameObject.Find("World").transform;
        obstaclePrefabs = new GameObject[obstacleNames.Length];
        obstacleProbabilities = new float[obstacleNames.Length];
        for (var i = 0; i < obstacleNames.Length; i++) {
            var obstacleName = obstacleNames[i];
            var obstaclePrefab = Resources.Load<GameObject>($"World/{obstacleName}");
            if (obstaclePrefab == null)
                throw new Exception("Missing Obstacle Prefab" + obstacleName);

            var probability = obstaclePrefab.GetComponent<Probability>();

            obstaclePrefabs[i] = obstaclePrefab;
            obstacleProbabilities[i] = probability.value;
        }

        await delay;

        for (var x = 0; x < Size.x; x++) {
            for (var y = 0; y < Size.y; y++) {
                var worldX = x - center.x;
                var worldZ = y - center.y;

                var point = new Vector3(worldX, 0, worldZ);
                var distance = Vector3.Distance(Vector3.zero, point);

                if (distance <= minRadius) continue;

                float probability;
                if (distance >= maxRadius)
                    probability = 1f;
                else
                    probability = Mathf.Lerp(0f, 1f, (distance - minRadius) / (maxRadius - minRadius));

                var random = Random.value;
                if (random > probability) continue;

                var prefab = ChooseObstaclePrefab();
                if (!prefab) continue;

                Object.Instantiate(prefab, point, Quaternion.identity, worldContainer);
            }
        }
    }

    private GameObject ChooseObstaclePrefab() {
        var randomNumber = Random.Range(0f, 100f);

        for (var i = 0; i < obstaclePrefabs.Length; i++) {
            randomNumber -= obstacleProbabilities[i];

            if (randomNumber <= 0f) return obstaclePrefabs[i];
        }

        return null;
    }
}
