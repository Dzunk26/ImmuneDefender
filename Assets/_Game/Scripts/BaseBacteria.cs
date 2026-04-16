using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBacteria : MonoBehaviour {
    public event EventHandler OnDeath;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;              // Tốc độ cơ bản

    [Header("Smooth Turning")]
    public float turnSpeed = 0.2f;            // Tốc độ xoay hướng (càng nhỏ càng chậm/mượt, thử 1.5-4.0)
    public float wanderStrength = 0.2f;       // Độ "lượn" mạnh (noise intensity)
    private int hp = 1;
    protected int trophicLevel = 0; //thu bac trong chuoi thuc an

    private float multiplicationTimer;
    private float multiplicationTimerMax = 10f;

    protected Rigidbody rb;
    private Vector3 currentDirection;
    private Vector3 desiredDirection;

    public virtual void TakeDamage(IAttackerStat attackerStat) {
        hp -= attackerStat.Damage;
        if (hp <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public virtual void Eat() { }

    public virtual void Eaten() { }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        currentDirection = UnityEngine.Random.onUnitSphere;
        currentDirection.y = 0f;
        currentDirection.Normalize();

        desiredDirection = currentDirection;
    }

    protected void HandleMovevement() {
        // Tạo noise thay đổi dần dần (dùng sin/cos để mượt, giống Perlin đơn giản)
        float time = Time.deltaTime;

        Vector3 noise = new Vector3(
            Mathf.Sin(time * 1.1f) * 0.6f + Mathf.Cos(time * 0.7f) * 0.4f,
            0f,
            Mathf.Sin(time * 0.9f) * 0.7f + Mathf.Cos(time * 1.3f) * 0.3f
        ) * wanderStrength;

        // Desired hướng thay đổi chậm theo noise
        desiredDirection += noise * time;
        desiredDirection.y = 0f;
        desiredDirection.Normalize();

        // Lerp current về desired → xoay mượt mà
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, turnSpeed * time);
        currentDirection.Normalize();

        // Áp dụng velocity
        Vector3 targetVelocity = currentDirection * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);

        // Xoay model vi khuẩn theo hướng di chuyển (mượt bằng Slerp)
        if (rb.velocity.sqrMagnitude > 0.01f) {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * time * 1.5f);
        }
    }

    protected void HanleMultiplication() {
        multiplicationTimer += Time.deltaTime;

        if (multiplicationTimer > multiplicationTimerMax) {
            // nhân đôi
        }
    }

    public int GetTrophicLevel() {
        return trophicLevel;
    }
}
