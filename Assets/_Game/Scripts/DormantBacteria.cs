using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the ngu dong de ne he mien dich tan cong
public class DormantBacteria : BaseBacteria {
    [SerializeField] private BacteriaSight bacteriaSight;
    [SerializeField] private Collider bodyCollider;

    private float dormancyTimer;
    private float dormancyTimerMax = 10f;
    private float dormancyChance = 0.5f;
    private bool isDormant = false;

    private void Start() {
        bacteriaSight.OnDangerDetected += BacteriaSight_OnDangerDetected;
    }

    private void BacteriaSight_OnDangerDetected(object sender, System.EventArgs e) {
        TryEnterDormancy();
    }

    private void Update() {
        if (isDormant) {
            HandleDormancy();
        }
        else {
            HanleMultiplication();
            HandleMovevement();
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

        if (Random.value < dormancyChance) {
            EnterDormancy();
        }
    }

    //vi khuan ngu dong
    private void EnterDormancy() {
        Debug.Log("Dormancy");
        isDormant = true;
        dormancyTimer = 0f;
        bodyCollider.enabled = false;
    }

    //vi khuan hoat dong tro lai
    private void Resuscitate() {
        isDormant = false;
        bodyCollider.enabled = true;
    }
}