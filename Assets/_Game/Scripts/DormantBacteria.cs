using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the ngu dong de ne he mien dich tan cong
public class DormantBacteria : BaseBacteria, IDormantable {
    public event EventHandler OnBecameUntargetable;
    public event EventHandler OnBecameTargetable;

    [SerializeField] private float dormancyTimerMax = 10f;
    [SerializeField] private float dormancyChance = 0.5f;

    private float dormancyTimer;

    public float DormancyTimerMax => dormancyTimerMax;
    public float DormancyChance => dormancyChance;
    public bool IsDormant { get; private set; } = false;

    private void BacteriaSight_OnDangerDetected(object sender, System.EventArgs e) {
        TryEnterDormancy();
    }

    public override void OnInit() {
        base.OnInit();
        IsDormant = false;
        dormancyTimer = 0f;
    }

    protected override void HandleStart() {
        bacteriaSight.OnDangerDetected += BacteriaSight_OnDangerDetected;
        base.HandleStart();
    }

    protected override void HandleUpdate() {
        if (IsDormant) {
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
        if (bacteriaState == BacteriaState.Hunt) return;
        if (IsDormant) return;

        if (UnityEngine.Random.value < dormancyChance) {
            EnterDormancy();
        }
    }

    //vi khuan ngu dong
    private void EnterDormancy() {
        IsDormant = true;
        dormancyTimer = 0f;
        bodyCollider.enabled = false;
        OnBecameUntargetable.Invoke(this, EventArgs.Empty);
    }

    //vi khuan hoat dong tro lai
    private void Resuscitate() {
        IsDormant = false;
        bodyCollider.enabled = true;
        OnBecameTargetable?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable() {
        bacteriaSight.OnDangerDetected -= BacteriaSight_OnDangerDetected;
    }
}