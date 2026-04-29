using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour, IAttackerStat {

    public int Damage => damage;
    public int Accuracy => accuracy;

    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private Collider attackCollider;

    private int damage = 3;
    private int accuracy = 200;
    private bool isAttacking = true;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseBacteria bacteria)) {
            Phagocytosis(bacteria);
        }
    }

    public void Phagocytosis(BaseBacteria bacteria) {
        if (!bacteria.IsHostile()) return;

        bacteria.Damage(this);
    }

    public bool CheckAttackState() {
        return isAttacking;
    }

    public void ActiveAttack() {
        isAttacking = true;
        attackCollider.enabled = true;
    }

    public void DeactiveAttack() {
        isAttacking = false;
        attackCollider.enabled = false;
    }
}