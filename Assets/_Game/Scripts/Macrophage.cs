using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : MonoBehaviour {
    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private Rigidbody rb;

    private float moveSpeed = 0.5f;

    private BaseBacteria currentTarget;

    private void Update() {
        currentTarget = macrophageSight.GetClosestBacteria(transform.position);
        HandleMovement();
    }

    private void HandleMovement() {
        Vector3 direction = new Vector3 (0, 0, 0);
        if (currentTarget != null) {
            direction = currentTarget.transform.position - transform.position;
        }
        else {

        }
        rb.velocity = direction * moveSpeed;
    }
}