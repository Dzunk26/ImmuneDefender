using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhagocyticCell : ImmuneCell<BaseBacteria>, IInfectableImmuneCell, IAttackerStat, IDamageable {
    public event EventHandler OnInfected;
    public event EventHandler OnApoptosis;

    [SerializeField] protected int damage;
    [SerializeField] protected int accuracy;

    public int Damage => damage;
    public int Accuracy => accuracy;
    public bool IsInfected { get; private set; }

    public void SetInfected(bool infected) {
        if (IsInfected) return;
        IsInfected = infected;
        if (infected) OnInfected?.Invoke(this, EventArgs.Empty);
    }

    public void Apoptosis() {
        OnApoptosis?.Invoke(this, EventArgs.Empty);
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }

    protected override void Attack() {
        if (!currentTarget.IsHostile()) return;
        currentTarget.TakeDamage(this);
        OnPhagocytosis();
    }

    public void TakeDamage(IAttackerStat attacker) {
        if (!IsInfected) return;

        Die();
    }
}