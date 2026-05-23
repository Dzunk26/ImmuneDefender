using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ImmuneCell<TTarget> : MonoBehaviour, IBuffable, ILimitedLifetime where TTarget : MonoBehaviour{
    public event EventHandler OnDeath;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] private float turnSpeed = 3f;

    [Header("Wander")]
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float waypointAngle = 120f;
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private float waypointTimerMax = 3f;

    [Header("Stats")]
    [SerializeField] private float aliveTimerMaxDefault = 60f;
    [SerializeField] private int phagocytosisLimitDefault = 8;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float updateTargetTimerMax = 1f;

    [SerializeField] protected string poolTag;

    private enum State { Wander, Chase }
    private State state = State.Wander;

    protected Vector3 currentWaypoint;
    private float waypointTimer;
    private float attackTimer;
    private float updateTargetTimer;

    protected TTarget currentTarget;

    public int PhagocytosisCount { get; private set; }
    public int PhagocytosisLimit { get; private set; }
    public float LifeTimer { get; private set; }
    public float LifeTimeLimit { get; private set; }

    private void OnEnable() {
        OnInit();
    }

    private void Update() {
        OnAlive();
        HandleState();
        UpdateTarget();
    }

    private void OnDisable() {
        OnDespawn();
    }

    protected abstract void Attack();

    protected abstract void RefreshTarget();

    protected virtual void OnInit() {
        LifeTimeLimit = UnityEngine.Random.Range(aliveTimerMaxDefault - 1f, aliveTimerMaxDefault + 1f);
        PhagocytosisLimit = phagocytosisLimitDefault;
        LifeTimer = 0f;
        PhagocytosisCount = 0;
        attackTimer = 0f;
        waypointTimer = 0f;
        updateTargetTimer = 0f;
        currentWaypoint = GetRandomWaypoint();

        HelperTHandler.Instance.Register(this);
    }

    protected virtual void OnDespawn() {
        HelperTHandler.Instance.Unregister(this);
    }

    private void UpdateTarget() {
        updateTargetTimer += Time.deltaTime;
        if (updateTargetTimer > updateTargetTimerMax) {
            updateTargetTimer = 0;
            RefreshTarget();
        }
    }

    private void HandleState() {
        state = currentTarget != null ? State.Chase : State.Wander;

        switch (state) {
            case State.Wander: HandleWander(); break;
            case State.Chase: HandleChase(); break;
        }
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
        HandleAttack(currentTarget);
    }

    private void HandleAttack(TTarget target) {
        float sqrDistance = (transform.position - target.transform.position).sqrMagnitude;
        if (sqrDistance > attackDistance * attackDistance) return;
        if (sqrDistance >= attackDistance * attackDistance) {
            LifeTimer += Time.deltaTime;
            return;
        }

        attackTimer = 0f;
        Attack();
    }

    protected void MoveToward(Vector3 targetPosition, float speed) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    protected Vector3 GetRandomWaypoint() {
        float randomAngle = UnityEngine.Random.Range(-waypointAngle * 0.5f, waypointAngle * 0.5f);
        Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y + randomAngle, 0f);
        Vector3 randomDirection = rotation * Vector3.forward;
        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);
        return transform.position + randomDirection * randomRadius;
    }

    public virtual void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    public void ApplyBuff(HelperTConfigSO config) {
        LifeTimeLimit += config.lifeTimeBonus;
        PhagocytosisLimit += config.phacytosisLimitBonus;
    }

    public void OnPhagocytosis() {
        PhagocytosisCount++;
        if (PhagocytosisCount >= PhagocytosisLimit) {
            Die();
        }
    }

    public void OnAlive() {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTimeLimit) {
            Die();
        }
    }
}