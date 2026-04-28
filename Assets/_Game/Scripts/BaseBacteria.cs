using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBacteria : MonoBehaviour, IDamageable {
    public event EventHandler OnDeath;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;              // Tốc độ cơ bản

    [Header("Smooth Turning")]
    public float turnSpeed = 0.2f;            // Tốc độ xoay hướng (càng nhỏ càng chậm/mượt, thử 1.5-4.0)
    [SerializeField] private float waypointDistance = 0.1f;  // khoảng cách coi là "đến nơi"
    [SerializeField] private float waypointRadius = 2f;      // bán kính random waypoint
    [SerializeField] private float waypointAngle = 120f;     // góc trước mặt để random
    [SerializeField] private float waypointTimerMax = 3f;    // thời gian tối đa trước khi random lại

    [SerializeField] private float aliveTimerMax = 20;
    [SerializeField] private float multiplicationTimerMax = 7f;

    [SerializeField] protected string poolTag;
    protected int trophicLevel = 0; // thu bac trong chuoi thuc an

    private float aliveTimer;
    private float multiplicationTimer;
    
    private int hp = 1;

    private Vector3 currentWaypoint;
    private float waypointTimer;

    public virtual void Damage(IAttackerStat attacker) {
        hp -= attacker.Damage;
        if (hp <= 0) {
            Die();
        }
    }

    public virtual bool IsHostile() => true; // danh dau vi khuan co hai hay khong

    public virtual void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        // thu vao pool
        Destroy(gameObject);
    }

    public virtual void Eat() { }

    public virtual void Eaten() { }

    private void Start() {
        currentWaypoint = GetRandomWaypoint();
    }

    private void Update() {
        HandleUpdate();
    }

    protected virtual void HandleUpdate() {
        HandleMovevement();
        HandleMultiplication(poolTag);
        SelfDestruct();
    }

    protected void HandleMovevement() {
        waypointTimer += Time.deltaTime;

        // Random waypoint mới nếu đến nơi hoặc hết thời gian
        bool arrivedAtWaypoint = Vector3.Distance(transform.position, currentWaypoint) < waypointDistance;
        bool waypointExpired = waypointTimer >= waypointTimerMax;

        if (arrivedAtWaypoint || waypointExpired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveTowardWaypoint();
    }

    protected void HandleMultiplication(string poolTag) {
        multiplicationTimer += Time.deltaTime;

        if (multiplicationTimer > multiplicationTimerMax) {
            GameObject newBacteria = ObjectPooler.Instance.GetFromPool(poolTag);
            if (newBacteria == null) return;

            //float randomAngle = UnityEngine.Random.Range(120f, 240f);
            //float childAngle = transform.eulerAngles.y + randomAngle;

            newBacteria.transform.position = transform.position;
            newBacteria.transform.rotation = transform.rotation;

            multiplicationTimer = 0f; // reset timer
        }
    }

    protected void SelfDestruct() {
        aliveTimer += Time.deltaTime;
        if (aliveTimer > aliveTimerMax) {
            Die();
        }
    }

    private void MoveTowardWaypoint() {
        Vector3 direction = (currentWaypoint - transform.position).normalized;
        direction.y = 0f;

        // Xoay mượt về hướng waypoint
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Di chuyển thẳng về phía trước
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z); // lock Y
    }

    private Vector3 GetRandomWaypoint() {
        // Random góc trong phạm vi ±60 độ trước mặt (tổng 120 độ)
        float halfAngle = waypointAngle * 0.5f;
        float randomAngle = UnityEngine.Random.Range(-halfAngle, halfAngle);

        // Xoay hướng hiện tại theo góc random
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 randomDirection = rotation * transform.forward;

        // Waypoint = vị trí hiện tại + hướng random * bán kính random
        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);

        return transform.position + randomDirection * randomRadius;
    }


    public int GetTrophicLevel() {
        return trophicLevel;
    }
}
