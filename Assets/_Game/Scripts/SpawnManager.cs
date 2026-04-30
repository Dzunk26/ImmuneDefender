using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager Instance { get; private set; }

    public event EventHandler OnInitPool;
    public event EventHandler OnSpawnObject;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        OnInitPool?.Invoke(this, EventArgs.Empty);
        OnSpawnObject?.Invoke(this, EventArgs.Empty);
    }
}