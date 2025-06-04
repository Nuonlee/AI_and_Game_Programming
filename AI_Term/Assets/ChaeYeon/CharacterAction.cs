using UnityEngine;
using System.Collections;

public class CharacterAction : MonoBehaviour
{
    [Header("References")]
    public AgentStatus agentStatus;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject swordOnShoulder;

    [Header("Movement")]
    public float speed = 5f;

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Rigidbody rigid;
    private Animator anim;

    private bool isMoving = false;
    private bool isEquipping = false;
    private bool isEquipped = false;
    private bool isAttacking = false;
    private bool isDefending = false;
    private bool isKicking = false;
    private bool isDodging = false;

    private int currentAttack = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // ---------- Movement ----------

    public void Move(Vector3 direction)
    {
        if (!IsAlive()) return;
        if (isEquipping || isDefending || isKicking || isAttacking) return;

        isMoving = true;
        moveVec = direction.normalized;

        if (isDodging) moveVec = dodgeVec;

        transform.position += moveVec * speed * Time.deltaTime;
        anim.SetBool("isMove", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
            transform.LookAt(transform.position + moveVec);
    }


    public void StopMove()
    {
        isMoving = false;
        anim.SetBool("isMove", false);
        moveVec = Vector3.zero;

    }


    public void MoveAwayFrom(Vector3 enemyPos, float duration = 1f)
    {
        if (isMoving) return;
        StartCoroutine(MoveAwayCoroutine(enemyPos, duration));
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

        if (isEquipping || isAttacking || isDefending || isKicking || isDodging)
            return;

        if (moveVec != Vector3.zero)
            return;

        isEquipping = true;
        anim.SetTrigger("doEquip");
    }

    public void ToggleWeapon()  // Animation Event용
    {
        Debug.Log("▶ ToggleWeapon 호출됨");
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
        StopMove();

        if (!IsAlive())
        {
            return false;
        }

        if (!isEquipped)
        {
            return false;
        }

        if (!agentStatus.CanAttack())
        {
            return false;
        }

        // 🥾 랜덤 발차기 (20% 확률)
        if (Random.value < 0.2f)
        {
            return KickAttack();
        }

        // ⚔️ 기존 칼 공격
        currentAttack++;

        if (currentAttack > 3)
            currentAttack = 1;

        if (Time.time - GetLastAttackTime() > 1.0f)
            currentAttack = 1;

        anim.SetTrigger("doAttack" + currentAttack);
        isAttacking = true;
        agentStatus.UseAttack();
        return true;
    }

    // ---------- Kick Attack ----------

    public bool KickAttack()
    {

        if (!IsAlive() || isKicking)
            return false;

        isKicking = true;
        anim.SetBool("isKick", true);
        agentStatus.UseAttack();

        return true;
    }

    private void EndKick() // Animation Event
    {
        isKicking = false;
        anim.SetBool("isKick", false);
    }

    public void ResetAttack()  // Animation Event
    {
        isAttacking = false;
    }

    public void ResetAttack1() => ResetAttack();
    public void ResetAttack2() => ResetAttack();
    public void ResetAttack3() => ResetAttack();



    // ---------- Defend ----------

    public bool TryDefend()
    {
        if (!IsAlive())
        {
            return false;
        }

        if (isMoving)
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

        return true;
    }


    public void ResetDefend()  // Animation Event용
    {
        isDefending = false;
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


    // ---------- Kick ----------


    public void Kick(bool isOn)
    {
        if (!IsAlive()) return;

        isKicking = isOn;
        anim.SetBool("isKick", isOn);
    }

    // ---------- Dodge ----------

    public bool TryDodge(Vector3 direction)
    {
        if (!IsAlive() || isDodging || !agentStatus.CanDodge())
            return false;

        StartCoroutine(DodgeRoutine(direction));
        return true;
    }

    private IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        dodgeVec = direction.normalized;
        agentStatus.UseDodge();
        anim.SetTrigger("doDodge");

        float dodgeDuration = 0.5f;
        float elapsed = 0f;
        float dodgeSpeed = speed * 1.5f;

        while (elapsed < dodgeDuration)
        {
            transform.position += dodgeVec * dodgeSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }

    // ---------- 상태 확인 메서드 ----------

    public bool IsAlive() => !agentStatus.isDead && agentStatus.currentHealth > 0f;
    public float GetLastAttackTime() => Time.time - agentStatus.GetLastAttackTime();

    public bool IsEquipping() => isEquipping;
    public bool IsEquipped() => isEquipped;
    public bool IsAttacking() => isAttacking;
    public bool IsBlocking() => isDefending;
    public bool IsKicking() => isKicking;
    public bool IsDodging() => isDodging;
}