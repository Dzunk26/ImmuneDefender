using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : MonoBehaviour, IDamageable {
    public event EventHandler OnDeath;

    [Header("Movement")]
    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] private float turnSpeed = 3f;

    [Header("Wander")]
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float waypointAngle = 120f;
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private float waypointTimerMax = 3f;

    private enum State { Wander, Chase }
    private State state = State.Wander;

    private BaseBacteria currentTarget;
    private Vector3 currentWaypoint;
    private float waypointTimer;
    private float updateTargetTimer;
    private float updateTargetTimerMax = 1f;
    private bool isInfected = false;

    public void Damage(IAttackerStat attacker) {
        if (!isInfected) return;

        Die();
    }

    private void Start() {
        macrophageSight.OnBacteriaListChange += MacrophageSight_OnBacteriaListChange;

        currentWaypoint = GetRandomWaypoint();
    }


    private void Update() {
        UpdateTarget();

        switch (state) {
            case 
                State.Wander: HandleWander(); 
                break;
            case 
                State.Chase: HandleChase(); 
                break;
        }
    }

    private void UpdateTarget() {
        updateTargetTimer += Time.deltaTime;
        if (updateTargetTimer > updateTargetTimerMax) {
            updateTargetTimer = 0;
            RefreshTarget();
            Debug.Log(currentTarget.gameObject);
        }
    }

    private void MacrophageSight_OnBacteriaListChange(object sender, System.EventArgs e) {
        RefreshTarget();
    }

    private void RefreshTarget() {
        currentTarget = macrophageSight.GetClosestBacteria(transform.position);
        state = currentTarget != null ? State.Chase : State.Wander;
    }

    private void HandleWander() {
        waypointTimer += Time.deltaTime;

        bool arrived = Vector3.Distance(transform.position, currentWaypoint) < waypointDistance;
        bool expired = waypointTimer >= waypointTimerMax;

        if (arrived || expired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveToward(currentWaypoint, moveSpeed);
    }

    private void HandleChase() {
        if (currentTarget == null) {
            state = State.Wander;
            return;
        }

        MoveToward(currentTarget.transform.position, chaseSpeed);
    }

    private void MoveToward(Vector3 targetPosition, float speed) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private Vector3 GetRandomWaypoint() {
        float halfAngle = waypointAngle * 0.5f;
        float randomAngle = UnityEngine.Random.Range(-halfAngle, halfAngle);

        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 randomDirection = rotation * transform.forward;

        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);

        return transform.position + randomDirection * randomRadius;
    }

    public void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void SetInfected(bool infected) {
        isInfected = infected;
    }

}