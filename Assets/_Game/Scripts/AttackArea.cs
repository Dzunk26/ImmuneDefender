using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour, IAttackerStat {

    public int Damage => damage;
    public int Accuracy => accuracy;

    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private Macrophage macrophage;
    [SerializeField] private int damage = 3;
    [SerializeField] private int accuracy = 200;
    [SerializeField] private float attackCoolDown = 0.5f;

    private bool isAttacking = true;
    private float attackTimer;
    private BaseBacteria currentTarget;

    public void HandleAttack(Vector3 fromPosition) {
        attackTimer += Time.deltaTime;
        if (attackTimer < attackCoolDown) {
            return;
        }

        currentTarget = macrophageSight.GetClosestBacteria(fromPosition);
    }

    public void Phagocytosis(BaseBacteria bacteria) {
        if (!bacteria.IsHostile()) return;

        bacteria.TakeDamage(this);
    }

    public bool CheckAttackState() {
        return isAttacking;
    }
}