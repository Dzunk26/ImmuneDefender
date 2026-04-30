using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoissonDiscSample : MonoBehaviour {
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int maxAttempts) {
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> activePoints = new List<Vector2>();

        activePoints.Add(sampleRegionSize / 2);
        while (activePoints.Count > 0) {
            int spawnIndex = Random.Range(0, activePoints.Count);
            Vector2 spawnPoint = activePoints[spawnIndex];
            bool newSpawnPointAccepted = false;

            for (int i = 0; i < maxAttempts; i++) {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 newSpawnPoint = spawnPoint + direction * Random.Range(radius, 2 * radius);
                if (IsValid(newSpawnPoint, sampleRegionSize, cellSize, radius, points, grid)) {
                    points.Add(newSpawnPoint);
                    activePoints.Add(newSpawnPoint);
                    grid[(int)(newSpawnPoint.x / cellSize), (int)(newSpawnPoint.y / cellSize)] = points.Count;
                    newSpawnPointAccepted = true;
                }
            }
            if (!newSpawnPointAccepted) {
                activePoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    private static bool IsValid(Vector2 newSpawnPoint, Vector2 sampleRegionsize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
        if (newSpawnPoint.x < 0 || newSpawnPoint.x > sampleRegionsize.x || newSpawnPoint.y < 0 || newSpawnPoint.y > sampleRegionsize.y) {
            return false;
        }
        int cellX = (int)(newSpawnPoint.x / cellSize);
        int cellY = (int)(newSpawnPoint.y / cellSize);

        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Max(grid.GetLength(0) - 1, cellX + 2);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Max(grid.GetLength(1) - 1, cellY + 2);

        for (int i = searchStartX; i < searchEndX; i++) {
            for (int j = searchStartY; j < searchEndY; j++) {
                int pointIndex = grid[i, j] - 1;
                if (pointIndex != -1) {
                    float sqrDistance = (newSpawnPoint - points[pointIndex]).sqrMagnitude;
                    if (sqrDistance < radius * radius) {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}