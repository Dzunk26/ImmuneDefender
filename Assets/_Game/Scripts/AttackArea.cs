using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour, IAttackerStat {
    public int Damage => damage;
    public int Accuracy => accuracy;

    [SerializeField] private MacrophageSight macrophageSight;

    private int damage = 3;
    private int accuracy = 200;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseBacteria bacteria)) {
            Phagocytosis(bacteria);
        }
    }

    private void Phagocytosis(BaseBacteria bacteria) {
        bacteria.TakeDamage(this);
    }
}