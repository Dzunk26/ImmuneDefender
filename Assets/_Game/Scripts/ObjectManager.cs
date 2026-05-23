using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    public static ObjectManager Instance { get; private set; }

    private List<BaseBacteria> allActiveBacterias = new List<BaseBacteria>();

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        for (int i = 0; i < allActiveBacterias.Count; i++) {
            allActiveBacterias[i].ManualUpdate();
        }
    }

    public void Register(BaseBacteria bacteria) {
        allActiveBacterias.Add(bacteria);
    }

    public void Unregister(BaseBacteria bacteria) {
        allActiveBacterias.Remove(bacteria);
    }

    public List<BaseBacteria> GetAllActiveBacterias() {
        return allActiveBacterias;
    }
}