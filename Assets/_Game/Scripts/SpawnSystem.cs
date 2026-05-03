using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour {
    [SerializeField] private float mapWidth = 1000f;
    [SerializeField] private float mapHeight = 1000f;
    private float safetyFactor = 0.9f; // he so an toan dam bao vi tao du so diem spawn vi khuan

    private void OnEnable() {
        SpawnManager.Instance.OnSpawnObjects += SpawnManager_OnSpawnObject;
    }

    private void SpawnManager_OnSpawnObject(object sender, SpawnManager.OnSpawnObjectsEventArg e) {
        Spawn(e.wave.listObjectSpawn);
    }

    private void Spawn(List<ObjectSpawnConfig> listObjectSpawn) {
        int totalCount = 0;
        foreach (ObjectSpawnConfig config in listObjectSpawn) {
            totalCount += config.amount;
        }

        float mapArea = mapWidth * mapHeight;
        float objectArea = mapArea / totalCount;
        float radius = Mathf.Sqrt(objectArea) * safetyFactor;

        List<Vector3> points = PoissonDiscSample.GeneratePoints(radius,new Vector2(mapWidth, mapHeight));
        ShuffleList(points);

        int pointIndex = 0;
        foreach (ObjectSpawnConfig objectSpawn in listObjectSpawn) {
            for (int i = 0; i < objectSpawn.amount; i++) {
                if (pointIndex >= points.Count) return;

                GameObject obj = ObjectPooler.Instance.GetFromPool(objectSpawn.poolTag);
                if (obj == null) continue;

                obj.transform.position = points[pointIndex];
                obj.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                pointIndex++;
            }
        }
    }

    private void ShuffleList(List<Vector3> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // swap
        }
    }

    private void OnDisable() {
        SpawnManager.Instance.OnSpawnObjects -= SpawnManager_OnSpawnObject;
    }
}
