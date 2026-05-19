using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : ImmuneCell<BaseBacteria>, IDamageable, IInfectable, IKillable, IAttackerStat {
    public event EventHandler OnApoptosis;
    public event EventHandler OnInfected;

    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private int damage = 3;
    [SerializeField] private int accuracy = 200;

    public bool IsInfected { get; private set; }
    public int Damage => damage;
    public int Accuracy => accuracy;

    private void Start() {
        macrophageSight.OnBacteriaListChange += MacrophageSight_OnBacteriaListChange;

        currentWaypoint = GetRandomWaypoint();
    }

    protected override void RefreshTarget() {
        currentTarget = macrophageSight.GetClosestBacteria(transform.position);
    }

    protected override void Attack() {
        if (!currentTarget.IsHostile()) return;
        currentTarget.TakeDamage(this);
    }

    private void MacrophageSight_OnBacteriaListChange(object sender, System.EventArgs e) {
        RefreshTarget();
    }

    public void TakeDamage(IAttackerStat attacker) {
        if (!IsInfected) return;

        Die();
    }

    public void Apoptosis() {
        OnApoptosis?.Invoke(this, EventArgs.Empty);
        ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    public void SetInfected(bool infected) {
        if (IsInfected) return;
        IsInfected = infected;
        if (infected) OnInfected?.Invoke(this, EventArgs.Empty);
    }
}