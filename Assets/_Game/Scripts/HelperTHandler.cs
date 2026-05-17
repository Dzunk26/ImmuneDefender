using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTHandler : MonoBehaviour {
    public static HelperTHandler Instance { get; private set; }

    private List<IBuffable> buffables = new List<IBuffable>();
    [SerializeField] private HelperTConfigSO config;

    private void Awake() {
        Instance = this;
    }

    public void Register (IBuffable buffable) {
        if (buffables.Contains(buffable)) return;

        buffables.Add(buffable);
    }

    public void Unregister(IBuffable buffable) {
        buffables.Remove(buffable);
    }

    public void ApplyHelperTBuff() {
        foreach (IBuffable buffable in buffables) {
            buffable.ApplyBuff(config);
        }
    }
}