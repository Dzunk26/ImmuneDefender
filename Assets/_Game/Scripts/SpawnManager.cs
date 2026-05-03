using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    private const string CURRENT_MAP_INDEX = "CurrentMapIndex";
    private const string CURRENT_WAVE_INDEX = "CurrentWaveIndex";

    public static SpawnManager Instance { get; private set; }

    public event EventHandler OnInitPool;
    public event EventHandler<OnSpawnObjectsEventArg> OnSpawnObjects;

    public class OnSpawnObjectsEventArg : EventArgs {
        public WaveConfigSO wave;
    }

    [SerializeField] List<MapConfigSO> mapConfigs;

    private MapConfigSO currentMap;
    private int currentMapIndex;
    private int currentWaveIndex;

    private void Awake() {
        Instance = this;

        currentMapIndex = PlayerPrefs.GetInt(CURRENT_MAP_INDEX, 0);
        currentWaveIndex = PlayerPrefs.GetInt(CURRENT_WAVE_INDEX, 0);
        currentMap = mapConfigs[currentMapIndex];
    }

    private void Start() {
        OnInitPool?.Invoke(this, EventArgs.Empty);
        OnSpawnObjects?.Invoke(this, new OnSpawnObjectsEventArg {
            wave = currentMap.waveConfigs[currentWaveIndex]
        });
    }

    public void SpawnCurrentWave() {
        if (currentWaveIndex >= currentMap.waveConfigs.Count) {
            Debug.Log("H?t wave");
            // chuyen map tiep theo va reset wave
            currentMapIndex++;
            currentWaveIndex = 0;
            return;
        }

        OnSpawnObjects?.Invoke(this, new OnSpawnObjectsEventArg {
            wave = currentMap.waveConfigs[currentWaveIndex - 1]
        });
    }
}