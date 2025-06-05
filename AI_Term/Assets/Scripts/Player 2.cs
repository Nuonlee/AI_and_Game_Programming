using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public GameObject sword;
    public GameObject swordOnShoulder;
    public GameObject attackPoint1;
    public GameObject attackPoint2;
    public GameObject attackPoint3;
    public Player1 player1;
    public GameManager gameManager;

    public float speed;
    public float maxHealth;
    public float curHealth;

    public float damage1;
    public float damage2;
    public float damage3;

    float hAxis;
    float vAxis;
    bool eDown;
    bool aDown;
    bool bDown;
    bool dDown;

    bool isEquipping;
    bool isEquipped;
    bool isAttack;
    bool isAttackCount;
    bool isBlock;
    bool isBlockCount;
    bool isDodge;
    bool isDamage;
    public bool isDead;

    public bool canAttack;
    public bool canBlock;
    public bool canDodge;

    float timeSinceAttack;
    public int currentAttack = 0;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (isAttackCount)
        {
            timeSinceAttack += Time.deltaTime;
        }
        else
        {
            timeSinceAttack = 0;
        }

        GetInput();
        Move();
        Turn();
        Equip();
        Attack();
        Block();
        Dodge();
    }

    // ------------------------------------------------ functions
    public void ApplyDamage21()
    {
        attackPoint1.SetActive(true);
    }

    void DisableCollider21()
    {
        attackPoint1.SetActive(false);
    }

    public void ApplyDamage22()
    {
        attackPoint2.SetActive(true);
    }

    void DisableCollider22()
    {
        attackPoint2.SetActive(false);
    }

    public void ApplyDamage23()
    {
        attackPoint3.SetActive(true);
    }

    void DisableCollider23()
    {
        attackPoint3.SetActive(false);
    }

    public void TakeDamage()
    {
        if (!isDodge && !isDead)
        {
            isDamage = true;

            if (!isBlock)
            {
                anim.SetTrigger("doDamage");
            }

            switch (player1.currentAttack)
            {
                case 1:
                    if (isBlock)
                    {
                        curHealth -= player1.damage1 / 10;
                    }
                    else
                    {
                        curHealth -= player1.damage1;
                    }
                    break;
                case 2:
                    if (isBlock)
                    {
                        curHealth -= player1.damage2 / 10;
                    }
                    else
                    {
                        curHealth -= player1.damage2;
                    }
                    break;
                case 3:
                    if (isBlock)
                    {
                        curHealth -= player1.damage3 / 10;
                    }
                    else
                    {
                        curHealth -= player1.damage3;
                    }
                    break;
            }

            if (curHealth <= 0)
            {
                curHealth = 0;
                Die();
            }

            Invoke("DamageOut", 0.5f);
        }
    }

    void DamageOut()
    {
        isDamage = false;

        if (isAttack)
        {
            isAttack = false;
            currentAttack = 0;
            gameManager.attack2Cool = 2.5f;
            gameManager.combo1Txt.gameObject.SetActive(false);
        }
    }

    void Die()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        gameManager.Result();
    }

    public void SwitchColliders2()
    {
        capsuleCollider.enabled = false;
        boxCollider.enabled = true;
    }

    // ------------------------------------------------ Update()
    void GetInput()
    {
        // Arrow Key
        hAxis = Input.GetAxisRaw("Horizontal2");
        vAxis = Input.GetAxisRaw("Vertical2");
        // U key -> 움직이고 있지 않을 때 검 장착
        eDown = Input.GetButtonDown("Equip2");
        // Q key -> 움직이고 있지 않고 검 장착 시 공격
        aDown = Input.GetButtonDown("Attack2");
        // I key -> 움직이고 있지 않을 때 방패 장착
        bDown = Input.GetButton("Block2");
        // Right Shift Key -> 움직이고 있을 때 그 방향으로 회피
        dDown = Input.GetButtonDown("Dodge2");
    }

    void Move()
    {
        if (isEquipping || isBlock || isAttack || isDamage || isDead)
        {
            return;
        }

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피 도중 방향을 바꾸지 않게 함
        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * Time.deltaTime;
        anim.SetBool("isMove", moveVec != Vector3.zero);
    }

    // 움직이는 방향으로 바라봄
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Equip()
    {
        if (eDown && !isBlock && moveVec == Vector3.zero)
        {
            isEquipping = true;
            anim.SetTrigger("doEquip");
        }
    }

    // Equip 2 애니메이션에 개입
    public void ActiveWeapon2()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = !isEquipped;
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = !isEquipped;
        }
    }

    public void Equipped2()
    {
        isEquipping = false;
    }

    void Attack()
    {
        if (aDown && !isBlock && canAttack && moveVec == Vector3.zero && (!isAttackCount || timeSinceAttack > 0.8f))
        {
            if (!isEquipped || isEquipping || isDead)
            {
                return;
            }

            currentAttack++;
            isAttack = true;
            isAttackCount = true;
            gameManager.combo2Txt.gameObject.SetActive(true);

            // 최대 3콤보
            if (currentAttack > 3)
            {
                currentAttack = 0;
                gameManager.attack2Cool = 2.5f;
                isAttackCount = false;
                gameManager.combo2Txt.gameObject.SetActive(false);
            }
            else
            {
                // 콤보에 따른 다른 애니메이션
                anim.SetTrigger("doAttack" + currentAttack);
            }

            // 타이머 리셋
            timeSinceAttack = 0;
        }

        if (timeSinceAttack > 1.0f)
        {
            currentAttack = 0;
            gameManager.attack2Cool = 2.5f;
            isAttackCount = false;
            gameManager.combo2Txt.gameObject.SetActive(false);
        }
    }

    // Attack2 관련 애니메이션에 개입
    public void ResetAttack2()
    {
        isAttack = false;
    }

    void Block()
    {
        if (bDown && !isAttack && canBlock && moveVec == Vector3.zero)
        {
            if (isEquipping || isDead)
            {
                return;
            }

            anim.SetBool("isBlock", true);
            isBlock = true;
            isBlockCount = true;
            gameManager.blocking2Txt.gameObject.SetActive(true);
        }
        else
        {
            anim.SetBool("isBlock", false);
            isBlock = false;
            gameManager.blocking2Txt.gameObject.SetActive(false);
            if (isBlockCount && !bDown)
            {
                gameManager.block2Cool = 2.5f;
                isBlockCount = false;
            }
        }
    }

    void Dodge()
    {
        if (dDown && canDodge && !isDodge && moveVec != Vector3.zero)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 0.5초 후에 회피 해제
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        gameManager.dodge2Cool = 5.0f;
        speed *= 0.5f;
        isDodge = false;
    }
}

