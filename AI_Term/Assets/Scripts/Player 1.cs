using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public GameObject sword;
    public GameObject swordOnShoulder;
    public GameObject attackPoint1;
    public GameObject attackPoint2;
    public GameObject attackPoint3;
    public Player2 player2;
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
    public void ApplyDamage11()
    {
        attackPoint1.SetActive(true);
    }

    void DisableCollider11()
    {
        attackPoint1.SetActive(false);
    }

    public void ApplyDamage12()
    {
        attackPoint2.SetActive(true);
    }

    void DisableCollider12()
    {
        attackPoint2.SetActive(false);
    }

    public void ApplyDamage13()
    {
        attackPoint3.SetActive(true);
    }

    void DisableCollider13()
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

            switch (player2.currentAttack)
            {
                case 1:
                    if (isBlock)
                    {
                        curHealth -= player2.damage1 / 10;
                    }
                    else
                    {
                        curHealth -= player2.damage1;
                    }
                        break;
                case 2:
                    if (isBlock)
                    {
                        curHealth -= player2.damage2 / 10;
                    }
                    else
                    {
                        curHealth -= player2.damage2;
                    }
                    break;
                case 3:
                    if (isBlock)
                    {
                        curHealth -= player2.damage3 / 10;
                    }
                    else
                    {
                        curHealth -= player2.damage3;
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
            gameManager.attack1Cool = 2.5f;
            gameManager.combo1Txt.gameObject.SetActive(false);
        }
    }

    void Die()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        gameManager.Result();
    }

    public void SwitchColliders1()
    {
        capsuleCollider.enabled = false;
        boxCollider.enabled = true;
    }

    // ------------------------------------------------ Update()
    void GetInput()
    {
        // WASD Key
        hAxis = Input.GetAxisRaw("Horizontal1");
        vAxis = Input.GetAxisRaw("Vertical1");
        // R key -> 움직이고 있지 않을 때 검 장착
        eDown = Input.GetButtonDown("Equip1");
        // P key -> 움직이고 있지 않고 검 장착 시 공격
        aDown = Input.GetButtonDown("Attack1");
        // E key -> 움직이고 있지 않을 때 방패 장착
        bDown = Input.GetButton("Block1");
        // Left Shift Key -> 움직이고 있을 때 그 방향으로 회피
        dDown = Input.GetButtonDown("Dodge1");
    }

    void Move()
    {
        if (isEquipping || isBlock || isAttack || isDead)
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

    // Equip 1 애니메이션에 개입
    public void ActiveWeapon1()
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

    public void Equipped1()
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
            gameManager.combo1Txt.gameObject.SetActive(true);

            // 최대 3콤보
            if (currentAttack > 3)
            {
                currentAttack = 0;
                gameManager.attack1Cool = 2.5f;
                isAttackCount = false;
                gameManager.combo1Txt.gameObject.SetActive(false);
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
            gameManager.attack1Cool = 2.5f;
            isAttackCount = false;
            gameManager.combo1Txt.gameObject.SetActive(false);
        }
    }

    // Attack1 관련 애니메이션에 개입
    public void ResetAttack1()
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
            gameManager.blocking1Txt.gameObject.SetActive(true);
        }
        else
        {
            anim.SetBool("isBlock", false);
            isBlock = false;
            gameManager.blocking1Txt.gameObject.SetActive(false);
            if (isBlockCount && !bDown)
            {
                gameManager.block1Cool = 2.5f;
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
        gameManager.dodge1Cool = 5.0f;
        speed *= 0.5f;
        isDodge = false;
    }
}
