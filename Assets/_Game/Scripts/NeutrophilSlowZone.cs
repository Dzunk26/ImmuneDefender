using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrophilSlowZone : MonoBehaviour {
    [SerializeField] private float duration;
    [SerializeField] private float slowMultiplier;
    [SerializeField] private string poolTag;

    private float durationTimer;

    private void OnEnable() {
        OnInit();
    }

    private void Update() {
        OnEnableZone();
    }

    private void OnInit() {
        durationTimer = 0f;
    }

    private void OnEnableZone() {
        durationTimer += Time.deltaTime;
        if (durationTimer > duration) {
            DisablZone();
        }
    }

    private void DisablZone() {
        ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out BaseBacteria bacteria)) {
            bacteria.ApplySlow(slowMultiplier);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out BaseBacteria bacteria)) {
            bacteria.RemoveSlow();
        }
    }
}