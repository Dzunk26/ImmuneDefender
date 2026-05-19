using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//te bao limpho T
public class KillerT : ImmuneCell<Macrophage> {
    [SerializeField] private KillerTSight killerTSight;

    private void Start() {
        killerTSight.OnTargetChange += KillerTSight_OnTargetChange;
        currentWaypoint = GetRandomWaypoint();
    }

    protected override void RefreshTarget() {
        currentTarget = killerTSight.GetClosestTarget(transform.position);
    }

    protected override void Attack() {
        OnPhagocytosis();
        currentTarget.Apoptosis();
    }

    private void KillerTSight_OnTargetChange(object sender, EventArgs e) {
        RefreshTarget();
    }
}
