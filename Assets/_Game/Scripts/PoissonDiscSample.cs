using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PoissonDiscSample {
    private static float adjustWidthSize = -500f;
    private static float adjustHeightSize = -500f;

    public static List<Vector3> GeneratePoints(float radius, Vector2 sampleRegionSize, int maxAttempts = 30) {
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector3> points = new List<Vector3>();
        List<Vector3> activePoints = new List<Vector3>();

        activePoints.Add(new Vector3(sampleRegionSize.x / 2, 0, sampleRegionSize.y / 2));
        while (activePoints.Count > 0) {
            int spawnIndex = Random.Range(0, activePoints.Count);
            Vector3 spawnPoint = activePoints[spawnIndex];
            bool newSpawnPointAccepted = false;

            for (int i = 0; i < maxAttempts; i++) {
                float angle = Random.value * Mathf.PI * 2;
                Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                Vector3 newSpawnPoint = spawnPoint + direction * Random.Range(radius, 2 * radius);
                if (IsValid(newSpawnPoint, sampleRegionSize, cellSize, radius, points, grid)) {
                    points.Add(newSpawnPoint);
                    activePoints.Add(newSpawnPoint);
                    grid[(int)(newSpawnPoint.x / cellSize), (int)(newSpawnPoint.z / cellSize)] = points.Count;
                    newSpawnPointAccepted = true;
                }
            }
            if (!newSpawnPointAccepted) {
                activePoints.RemoveAt(spawnIndex);
            }
        }
        AdjustPoints(points);

        return points;
    }

    private static bool IsValid(Vector3 newSpawnPoint, Vector2 sampleRegionsize, float cellSize, float radius, List<Vector3> points, int[,] grid) {
        if (newSpawnPoint.x < 0 || newSpawnPoint.x > sampleRegionsize.x || newSpawnPoint.z < 0 || newSpawnPoint.z > sampleRegionsize.y) {
            return false;
        }
        int cellX = (int)(newSpawnPoint.x / cellSize);
        int cellZ = (int)(newSpawnPoint.z / cellSize);

        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(grid.GetLength(0) - 1, cellX + 2);
        int searchStartZ = Mathf.Max(0, cellZ - 2);
        int searchEndZ = Mathf.Min(grid.GetLength(1) - 1, cellZ + 2);

        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int z = searchStartZ; z <= searchEndZ; z++) {
                int pointIndex = grid[x, z] - 1;
                if (pointIndex != -1 && pointIndex < points.Count) {
                    float sqrDistance = (newSpawnPoint - points[pointIndex]).sqrMagnitude;
                    if (sqrDistance < radius * radius) {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private static void AdjustPoints(List<Vector3> points) {
        for (int i = 0; i < points.Count; i++) {
            points[i] += new Vector3(adjustWidthSize, 0, adjustHeightSize);
        }
    }
}