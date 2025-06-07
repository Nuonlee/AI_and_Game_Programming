using UnityEngine;
using System;

public class AgentStatus : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Attack")]
    public float attackDamage = 10f;

    [Header("Cooldowns (seconds)")]
    public float attackCooldown = 2.5f;
    public float defendCooldown = 2.5f;
    public float dodgeCooldown = 5f;

    private float lastAttackTime;
    private float lastDefendTime;
    private float lastDodgeTime;

    [Header("Invincibility")]
    public bool isInvincible = false;
    public float invincibilityDuration = 0.5f;
    private float invincibleEndTime;

    [Header("Flags")]
    public bool isBeingAttacked = false;
    public bool isDefending = false;
    public bool isDead = false; 
    private bool hasDefended = false;

    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = -attackCooldown;
        lastDefendTime = -defendCooldown;
        lastDodgeTime = -dodgeCooldown;
    }

    void Update()
    {
        if (isInvincible && Time.time >= invincibleEndTime)
            isInvincible = false;
    }

    // --------- 데미지 및 죽음 ---------

    public void TakeDamage(float damage)
    {
        if (isInvincible || isDead) return;

        if (isDefending)
        {
            isDefending = false;
            damage *= 0.2f;
        }
           

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // 추가
        isBeingAttacked = true;
        CancelInvoke(nameof(ResetAttacked)); // 중복 방지
        Invoke(nameof(ResetAttacked), 0.5f);  // 유지시간 조절 가능

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Die");

        BTAgentBase agent = GetComponent<BTAgentBase>();
        if (agent != null)
            agent.OnDeath();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = Vector3.zero;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        OnDeath?.Invoke();

        Destroy(gameObject, 3f);
    }

    // --------- 무적 ---------

    public void StartInvincibility()
    {
        isInvincible = true;
        invincibleEndTime = Time.time + invincibilityDuration;
    }

    // --------- 쿨타임 체크 ---------

    public bool CanAttack() => Time.time >= lastAttackTime + attackCooldown;
    public bool CanDefend() => Time.time >= lastDefendTime + defendCooldown;
    public bool CanDodge() => Time.time >= lastDodgeTime + dodgeCooldown;

    public void UseAttack() => lastAttackTime = Time.time;
    public void UseDefend() => lastDefendTime = Time.time;
    public void UseDodge()
    {
        lastDodgeTime = Time.time;
        StartInvincibility();
    }

    public float GetLastAttackTime() => lastAttackTime;

    // --------- 상태 확인 ---------

    public bool IsAlive() => !isDead && currentHealth > 0f;

    // --------- 피격 감지 ---------

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            isBeingAttacked = true;
            Invoke(nameof(ResetAttacked), 0.5f);
            TakeDamage(attackDamage);
        }
    }

    void ResetAttacked()
    {
        isBeingAttacked = false;
    }
}