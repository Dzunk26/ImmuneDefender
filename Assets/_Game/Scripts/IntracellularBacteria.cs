using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the song ky sinh trong dai thuc bao
public class IntracellularBacteria : BaseBacteria, IIntracelullarable {
    public event EventHandler OnBecameUntargetable;
    public event EventHandler OnBecameTargetable;

    private IInfectableImmuneCell currentHost;

    public bool IsParasiting { get; private set; }

    private void OnTriggerEnter(Collider other) {
        if (IsParasiting) return;
        if (other.TryGetComponent(out IInfectableImmuneCell infcetable)) {
            EnterHost(infcetable);
        }
    }

    public override void OnInit() {
        base.OnInit();
        currentHost = null;
        IsParasiting = currentHost != null;
    }

    protected override void HandleUpdate() {
        HandleMultiplication(poolTag);
        HandleUpdateActivityLevel();
        SelfDestruct();

        if (!IsParasiting) {
            HandleMovevement();
            HandleState();
        }
    }

    public void EnterHost(IInfectableImmuneCell host) {
        currentHost = host;
        currentHost.OnDeath += PhagocyticCell_OnDeath;
        currentHost.OnApoptosis += PhagocyticCell_OnApoptosis;
        currentHost.SetInfected(true);

        transform.SetParent((host as MonoBehaviour).transform);
        transform.localPosition = Vector3.zero;

        bacteriaVisual.SetActive(false);
        bodyCollider.enabled = false;
        OnBecameUntargetable?.Invoke(this, EventArgs.Empty);
    }

    private void PhagocyticCell_OnApoptosis(object sender, EventArgs e) {
        UnsubscribePhagocyticCell();
        Die();
    }

    private void PhagocyticCell_OnDeath(object sender, EventArgs e) {
        UnsubscribePhagocyticCell();
        ExitHost();
    }

    private void UnsubscribePhagocyticCell() {
        currentHost.OnDeath -= PhagocyticCell_OnDeath;
        currentHost.OnApoptosis -= PhagocyticCell_OnApoptosis;
    }

    private void ExitHost() {
        currentHost.SetInfected(false);
        transform.SetParent(null);

        bacteriaVisual.SetActive(true);
        bodyCollider.enabled = true;
        currentHost = null;
        OnBecameTargetable?.Invoke(this, EventArgs.Empty);
    }
}