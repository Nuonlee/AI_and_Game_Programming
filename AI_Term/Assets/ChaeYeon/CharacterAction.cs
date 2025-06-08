using UnityEngine;
using System.Collections;

public class CharacterAction : MonoBehaviour
{
    [Header("References")]
    public AgentStatus agentStatus;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject swordOnShoulder;
    [SerializeField] Collider swordCollider;

    [Header("Movement")]
    public float speed = 5f;

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
    private bool successfulAttack = false;


    public void OnBlockSuccess()
    {
        DidBlockSuccessfully = true;
    }
    public void ResetBlockStatus()
    {
        DidBlockSuccessfully = false;
    }
    public void OnSuccessfulAttack()
    {
        successfulAttack = true;
    }

    public bool WasSuccessfulAttack()
    {
        bool result = successfulAttack;
        successfulAttack = false;  // 리셋
        return result;
    }
    public bool WasSuccessfulBlock()
    {
        bool result = DidBlockSuccessfully;
        DidBlockSuccessfully = false;
        return result;
    }



    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // ---------- Movement ----------

    public void Move(Vector3 direction)
    {
        if (!IsAlive()) return;
        if (isEquipping || isDefending || isAttacking || isDodging) return;

        Vector3 normalizedDir = direction.normalized;

        if (isDodging)
            normalizedDir = dodgeVec;

        // 위치 이동
        transform.position += normalizedDir * speed * Time.deltaTime;

        // moveVec이 바뀐 경우에만 SetBool 호출
        if (!isMoving || moveVec != normalizedDir)
        {
            anim.SetBool("isMove", true);
            isMoving = true;
        }

        moveVec = normalizedDir;

        // 회전
        if (moveVec != Vector3.zero)
            transform.LookAt(transform.position + moveVec);
    }


    public void StopMove()
    {
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
        Vector3 lookPos = targetPosition;
        lookPos.y = transform.position.y;  // Y축 회전만 적용 (고개 들거나 숙이지 않게)

        transform.LookAt(lookPos);
    }



    // ---------- Equip ----------

    public void EquipWeapon()
    {
        if (!IsAlive()) return;

        if (isEquipping || isAttacking || isDodging)
            return;

        if (moveVec != Vector3.zero)
            return;

        isEquipping = true;
        anim.SetTrigger("doEquip");
    }

    public void ToggleWeapon()  // Animation Event용
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

    public void OnEquipFinished()  // Animation Event용
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

    public void TryComboAttack()
    {
        float comboChance = 0.3f; // 30% 확률

        if (Random.value < comboChance)
        {
            isAttacking = true;
            anim.SetTrigger("doAttack" + 3);
            agentStatus.UseAttack();
        }
        else
            ResetAttack1();
    }



    public void CounterAttack()  // 수비 에이전트 반격용
    {
       // StopMove();

        if (!IsAlive() || isEquipping || isDodging)
        {
            Debug.Log("💥 isDefending?");
            return;
        }

        swordCollider.enabled = true;

        anim.SetTrigger("doAttack" + 2);
        isAttacking = true;

    }


    public void ResetAttack1()  // Animation Event
    {
        isAttacking = false;
      //  swordCollider.enabled = false;
    }



    // ---------- Defend ----------

    public bool TryDefend()
    {
        if ( !IsAlive() || isDodging || isMoving || isAttacking || isDefending)
        {
            return false;
        }

        if (!agentStatus.CanDefend())
        {
            return false;
        }

        isDefending = true;
        agentStatus.isDefending = true;
        anim.SetTrigger("doBlock");
        agentStatus.UseDefend();

        Invoke(nameof(ResetDefend), 0.5f);

        return true;
    }


    public void ResetDefend()  // Animation Event용
    {
        isDefending = false;
        agentStatus.isDefending = false;
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



    // ---------- Dodge ----------

    public bool TryDodge(Vector3 direction)
    {
        //Debug.Log($"[TryDodge] 상태 체크 → isAttacking: {isAttacking}, isDefending: {isDefending}, isDodging: {isDodging}, CanDodge: {agentStatus.CanDodge()}");

        if (!IsAlive() || !agentStatus.CanDodge() || isDodging || isAttacking || isEquipping || isDefending )
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
        Vector3 dodgeDir = Random.value < 0.5f ? right : -right;
        dodgeVec = dodgeDir.normalized;

        float dodgeDuration = 0.5f;
        float elapsed = 0f;
        float dodgeSpeed = speed * 1.5f;

        while (elapsed < dodgeDuration)
        {
            transform.position += dodgeVec * dodgeSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        TurnTo(direction);
        isDodging = false;
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