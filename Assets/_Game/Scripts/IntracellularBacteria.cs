using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the song ky sinh trong dai thuc bao
public class IntracellularBacteria : BaseBacteria, IUntargetable {
    public event EventHandler OnBecameUntargetable;
    public event EventHandler OnBecameTargetable;

    private Macrophage currentHost;

    private bool isParasiting => currentHost != null;

    private void OnTriggerEnter(Collider other) {
        if (isParasiting) return;
        if (other.TryGetComponent(out Macrophage macrophage)) {
            EnterHost(macrophage);
        }
    }

    protected override void HandleUpdate() {
        HandleMultiplication(poolTag);
        HandleUpdateActivityLevel();
        SelfDestruct();

        if (!isParasiting) {
            HandleMovevement();
        }
    }

    public void EnterHost(Macrophage host) {
        currentHost = host;
        currentHost.OnDeath += Host_OnDeath;
        currentHost.SetInfected(true);

        transform.SetParent(host.transform);
        transform.localPosition = Vector3.zero;

        bacteriaVisual.SetActive(false);
        bodyCollider.enabled = false;
        OnBecameUntargetable?.Invoke(this, EventArgs.Empty);
    }

    private void Host_OnDeath(object sender, EventArgs e) {
        currentHost.OnDeath -= Host_OnDeath;
        ExitHost();
    }

    private void ExitHost() {
        currentHost.OnDeath -= Host_OnDeath;
        currentHost.SetInfected(false);

        transform.SetParent(null);

        bacteriaVisual.SetActive(true);
        bodyCollider.enabled = true;
        currentHost = null;
        OnBecameTargetable?.Invoke(this, EventArgs.Empty);
    }
}