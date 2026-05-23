using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutrophil : PhagocyticCell {
    [SerializeField] private NeutrophilSight neutrophilSight;
    [SerializeField] private string slowZonePoolTag;

    private void Start() {
        neutrophilSight.OnBacteriaListChange += NeutrophilSight_OnBacteriaListChange;
    }

    private void NeutrophilSight_OnBacteriaListChange(object sender, EventArgs e) {
        RefreshTarget();
    }

    private void SpawnSLowZone() {

    }

    protected override void Attack() {
        if (!currentTarget.IsHostile()) return;
        currentTarget.TakeDamage(this);
    }

    protected override void RefreshTarget() {
         currentTarget = neutrophilSight.GetClosestBacteria(transform.position);
    }

    public override void Die() {
        SpawnSLowZone();
        base.Die();
    }
}