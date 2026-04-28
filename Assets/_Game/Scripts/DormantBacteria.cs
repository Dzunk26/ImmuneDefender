using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the ngu dong de ne he mien dich tan cong
public class DormantBacteria : BaseBacteria, IUntargetable {
    public event EventHandler OnBecameUntargetable;
    public event EventHandler OnBecameTargetable;

    [SerializeField] private BacteriaSight bacteriaSight;
    [SerializeField] private Collider bodyCollider;

    private float dormancyTimer;
    private float dormancyTimerMax = 10f;
    private float dormancyChance = 0.5f;
    private bool isDormant = false;

    private void OnEnable() {
        bacteriaSight.OnDangerDetected += BacteriaSight_OnDangerDetected;
    }

    private void BacteriaSight_OnDangerDetected(object sender, System.EventArgs e) {
        TryEnterDormancy();
    }

    protected override void HandleUpdate() {
        if (isDormant) {
            HandleDormancy();
        }
        else {
            base.HandleUpdate();
        }
    }

    private void HandleDormancy() {
        dormancyTimer += Time.deltaTime;
        if (dormancyTimer > dormancyTimerMax) {
            Resuscitate();
        }
    }

    private void TryEnterDormancy() {
        if (isDormant) return;

        if (UnityEngine.Random.value < dormancyChance) {
            EnterDormancy();
        }
    }

    //vi khuan ngu dong
    private void EnterDormancy() {
        Debug.Log("Dormancy");
        isDormant = true;
        dormancyTimer = 0f;
        bodyCollider.enabled = false;
        OnBecameUntargetable.Invoke(this, EventArgs.Empty);
    }

    //vi khuan hoat dong tro lai
    private void Resuscitate() {
        isDormant = false;
        bodyCollider.enabled = true;
        OnBecameTargetable?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable() {
        bacteriaSight.OnDangerDetected -= BacteriaSight_OnDangerDetected;
    }
}