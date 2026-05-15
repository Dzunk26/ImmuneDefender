using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerTSight : MonoBehaviour {
    public event EventHandler OnTargetChange;

    private List<Macrophage> infectedInRange = new List<Macrophage>();
    private Macrophage cachedClosestTarget;

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Macrophage macrophage)) return;

        macrophage.OnDeath += Macrophage_OnDeath;

        if (macrophage.IsInfected) {
            infectedInRange.Add(macrophage);
            UpdateTargetState();
            return;
        }

        macrophage.OnInfected += Macrophage_OnInfected;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out Macrophage macrophage)) return;
        RemoveMacrophage(macrophage);
    }

    private void Macrophage_OnInfected(object sender, EventArgs e) {
        Macrophage macrophage = sender as Macrophage;
        macrophage.OnInfected -= Macrophage_OnInfected;
        infectedInRange.Add(macrophage);
        UpdateTargetState();
    }

    private void Macrophage_OnDeath(object sender, EventArgs e) {
        Macrophage macrophage = sender as Macrophage;
        RemoveMacrophage(macrophage);
    }

    private void RemoveMacrophage(Macrophage macrophage) {
        macrophage.OnInfected -= Macrophage_OnInfected;
        macrophage.OnDeath -= Macrophage_OnDeath;
        infectedInRange.Remove(macrophage);
        UpdateTargetState();
    }

    private void UpdateTargetState() {
        cachedClosestTarget = GetClosestTarget(transform.position);
        OnTargetChange?.Invoke(this, EventArgs.Empty);
    }

    public Macrophage GetClosestTarget(Vector3 fromPosition) {
        Macrophage closest = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (Macrophage macrophage in infectedInRange) {
            float sqrDistance = (fromPosition - macrophage.transform.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance) {
                minSqrDistance = sqrDistance;
                closest = macrophage;
            }
        }
        return closest;
    }

    public Macrophage GetCachedClosestTarget() {
        return cachedClosestTarget;
    }
}