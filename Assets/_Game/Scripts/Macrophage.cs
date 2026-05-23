using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : PhagocyticCell {
    [SerializeField] private MacrophageSight macrophageSight;

    private void Start() {
        macrophageSight.OnBacteriaListChange += MacrophageSight_OnBacteriaListChange;

        currentWaypoint = GetRandomWaypoint();
    }

    private void MacrophageSight_OnBacteriaListChange(object sender, System.EventArgs e) {
        RefreshTarget();
    }
    protected override void RefreshTarget() {
        currentTarget = macrophageSight.GetClosestBacteria(transform.position);
    }

    protected override void Attack() {
        if (!currentTarget.IsHostile()) return;
        currentTarget.TakeDamage(this);
    }
}