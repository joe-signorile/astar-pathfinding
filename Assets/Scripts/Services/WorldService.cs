using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WorldService {
    private readonly WorldConfig worldConfig;
    private readonly Transform worldContainer;

    public WorldService() {
        worldConfig = new WorldConfig();
        worldContainer = GameObject.Find("World").transform;
    }

    public async Task Start() {
        var obstacles = worldConfig.Obstacles;
        var center = new float2(worldConfig.Size.x * 0.5f, worldConfig.Size.y * 0.5f);
        var delay = Task.Yield();

        var obstaclePrefabs = new GameObject[obstacles.Length];
        var obstacleProbabilities = new float[obstacles.Length];
        for (var i = 0; i < obstacles.Length; i++) {
            var obstacle = obstacles[i];
            var obstacleName = obstacle.Id;
            var probability = obstacle.Probability;

            var obstaclePrefab = Resources.Load<GameObject>($"World/{obstacleName}");
            if (obstaclePrefab == null)
                throw new Exception("Missing Obstacle Prefab" + obstacleName);

            obstaclePrefabs[i] = obstaclePrefab;
            obstacleProbabilities[i] = probability;
        }

        await delay;

        var minRadius = worldConfig.MinRadius;
        var maxRadius = worldConfig.MaxRadius;
        for (var x = 0; x < worldConfig.Size.x; x++) {
            for (var y = 0; y < worldConfig.Size.y; y++) {
                var worldX = x - center.x;
                var worldZ = y - center.y;

                var point = new Vector3(worldX, 0, worldZ);
                var distance = Vector3.Distance(Vector3.zero, point);

                if (distance <= minRadius) continue;

                var probability = distance >= maxRadius ? 1f : Mathf.Lerp(0f, 1f, (distance - minRadius) / (maxRadius - minRadius));
                var random = Random.value;
                if (random > probability) continue;

                var prefab = ChooseObstaclePrefab(obstacleProbabilities, obstaclePrefabs);
                if (!prefab) continue;

                Object.Instantiate(prefab, point, Quaternion.identity, worldContainer);
            }
        }
    }

    private GameObject ChooseObstaclePrefab(float[] probabilities, GameObject[] prefabs) {
        var randomNumber = Random.Range(0f, 100f);

        for (var i = 0; i < probabilities.Length; i++) {
            randomNumber -= probabilities[i];

            if (randomNumber <= 0f) return prefabs[i];
        }

        return null;
    }
}
