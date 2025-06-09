using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterAction_Re : MonoBehaviour
{
    [Header("References")]
    public AgentStatus_Re agentStatus;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject swordOnShoulder;
    [SerializeField] Collider swordCollider;

    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Rigidbody rigid;
    private Animator anim;

    [Header("Flag")]
    public bool DidBlockSuccessfully { get; set; }
    public bool isAttacking = false;
    private bool isMoving = false;
    private bool isEquipping = false;
    private bool isEquipped = false;
    private bool isDefending = false;
    private bool isDodging = false;

    private int currentAttack = 0;
    public bool isResetting = false;

    public void OnBlockSuccess()
    {
        DidBlockSuccessfully = true;
    }

    public void ResetBlockStatus()
    {
        DidBlockSuccessfully = false;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 수평 회전만 허용
    }

    void FixedUpdate()
    {
        if (moveVec != Vector3.zero && !isDodging)
        {
            Vector3 moveTarget = transform.position + moveVec * speed * Time.fixedDeltaTime;
            rigid.MovePosition(moveTarget);

            // 부드러운 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            rigid.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // ---------- Movement ----------

    public void Move(Vector3 direction)
    {
        if (!IsAlive() || isEquipping || isDefending || isAttacking || isDodging || isResetting) return;

        Vector3 normalizedDir = direction.normalized;
        moveVec = normalizedDir;

        if (!isMoving && moveVec != Vector3.zero)
        {
            anim.SetBool("isMove", true);
            isMoving = true;
        }

        if (moveVec == Vector3.zero && isMoving)
        {
            StopMove();
        }
    }

    public void StopMove()
    {
        moveVec = Vector3.zero;
        anim.SetBool("isMove", false);
        isMoving = false;
    }

    private IEnumerator MoveAwayCoroutine(Vector3 enemyPos, float duration)
    {
        isMoving = true;
        float timer = 0f;

        Vector3 awayDir = (transform.position - enemyPos).normalized;

        while (timer < duration)
        {
            transform.position += awayDir * speed * 2f * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        TurnTo(enemyPos);
        isMoving = false;
    }

    public void TurnTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;

        if (direction.magnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // ---------- Equip ----------

    public void EquipWeapon()
    {
        if (!IsAlive()) return;
        if (isEquipping || isAttacking || isDodging) return;
        if (moveVec != Vector3.zero) return;

        isEquipping = true;
        anim.SetTrigger("doEquip");
    }

    public void ToggleWeapon()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = true;
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = false;
        }
    }

    public void OnEquipFinished()
    {
        isEquipping = false;
    }

    // ---------- Attack ----------

    public bool TryAttack()
    {
        //StopMove();

        if (!IsAlive() || isEquipping || isDefending || isDodging || isAttacking)
        {
            //Debug.Log("❌ AttackAction: 공격 실패 - 아니면 isDefending ??????" + isDefending);

            //        Debug.Log("❌ AttackAction: 공격 실패 - 아니면 isattack ??????" + isEquipping);
            return false;
        }


        if (!agentStatus.CanAttack())
        {
            //         Debug.Log("❌ AttackAction: 공격 실패 - CanAttack ??????");
            return false;
        }

        swordCollider.enabled = true;
        currentAttack++;

        currentAttack = 1;
        //if (currentAttack > 3)
        //    currentAttack = 1;

        //if (Time.time - GetLastAttackTime() > 1.0f)
        //    currentAttack = 1;

        anim.SetTrigger("doAttack" + currentAttack);
        isAttacking = true;
        agentStatus.UseAttack();
        return true;
    }

    public void ResetAttack1()
    {
        isAttacking = false;
        swordCollider.enabled = false;
    }

    public void TryComboAttack()
    {
        float comboChance = 0.3f;
        if (Random.value < comboChance)
        {
            isAttacking = true;
            anim.SetTrigger("doAttack" + 3);
            agentStatus.UseAttack();
        }
        else ResetAttack1();
    }

    public void CounterAttack()
    {
        if (!IsAlive() || isEquipping || isDodging) return;

        swordCollider.enabled = true;
        anim.SetTrigger("doAttack" + 2);
        isAttacking = true;
    }

    // ---------- Dash ----------

    public void DashTo(Vector3 targetPos, float duration = 0.5f)
    {
        if (isMoving) return;
        StartCoroutine(DashCoroutine(targetPos, duration));
    }

    private IEnumerator DashCoroutine(Vector3 targetPos, float duration)
    {
        isMoving = true;

        anim.SetTrigger("doDodge");
        Vector3 start = transform.position;
        Vector3 direction = (targetPos - start).normalized;

        float timer = 0f;
        while (timer < duration)
        {
            transform.position += direction * speed * 1.5f * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
    // ---------- Defend ----------

    public bool TryDefend()
    {
        if (!IsAlive() || isDodging || isMoving || isAttacking || isDefending) return false;
        if (!agentStatus.CanDefend()) return false;

        isDefending = true;
        agentStatus.isDefending = true;
        anim.SetTrigger("doBlock");
        agentStatus.UseDefend();

        Invoke(nameof(ResetDefend), 0.5f);
        return true;
    }

    public void ResetDefend()
    {
        isDefending = false;
        agentStatus.isDefending = false;
    }

    // ---------- Dodge ----------

    public bool TryDodge(Vector3 direction)
    {
        if (!IsAlive() || !agentStatus.CanDodge() || isDodging || isAttacking || isEquipping || isDefending)
            return false;

        StartCoroutine(DodgeRoutine(direction));
        return true;
    }

    private IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        agentStatus.UseDodge();
        anim.SetTrigger("doDodge");

        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
        dodgeVec = (Random.value < 0.5f ? right : -right).normalized;

        float dodgeDuration = 0.5f;
        float elapsed = 0f;
        float dodgeSpeed = speed * 1.5f;

        while (elapsed < dodgeDuration)
        {
            rigid.MovePosition(rigid.position + dodgeVec * dodgeSpeed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        TurnTo(direction);
        isDodging = false;
    }

    public void ResetStateFlags()
    {
        isAttacking = false;
        isDodging = false;
        isEquipping = false;
        isDefending = false;
    }

    // ---------- 상태 확인 메서드 ----------

    public bool IsAlive() => !agentStatus.isDead && agentStatus.currentHealth > 0f;
    public float GetLastAttackTime() => Time.time - agentStatus.GetLastAttackTime();

    public bool IsEquipping() => isEquipping;
    public bool IsEquipped() => isEquipped;
    public bool IsAttacking() => isAttacking;
    public bool IsBlocking() => isDefending;
    public bool IsDodging() => isDodging;
}
