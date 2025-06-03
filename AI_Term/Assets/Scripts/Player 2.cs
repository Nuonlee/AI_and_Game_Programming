using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public GameObject sword;
    public GameObject swordOnShoulder;
    public Player1 player1;
    public GameObject attackPoint1;
    public GameObject attackPoint2;
    public GameObject attackPoint3;

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
    bool isBlock;
    bool isDodge;

    float timeSinceAttack;
    public int currentAttack = 0;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;

        GetInput();
        Move();
        Turn();
        Equip();
        Attack();
        Block();
        Dodge();
    }

    // ------------------------------------------------ public
    public void ApplyDamage1()
    {
        attackPoint1.SetActive(true);
    }

    void DisableCollider1()
    {
        attackPoint1.SetActive(false);
    }

    public void ApplyDamage2()
    {
        attackPoint2.SetActive(true);
    }

    void DisableCollider2()
    {
        attackPoint2.SetActive(false);
    }

    public void ApplyDamage3()
    {
        attackPoint3.SetActive(true);
    }

    void DisableCollider3()
    {
        attackPoint3.SetActive(false);
    }

    public void TakeDamage()
    {
        if (!isBlock && !isDodge)
        {
            switch (player1.currentAttack)
            {
                case 1:
                    curHealth -= player1.damage1;
                    break;
                case 2:
                    curHealth -= player1.damage2;
                    break;
                case 3:
                    curHealth -= player1.damage3;
                    break;
            }

            if (curHealth <= 0)
            {
                curHealth = 0;
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Player2 died!");
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
        if (isEquipping || isBlock || isAttack)
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
        if (eDown && moveVec == Vector3.zero)
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
        if (aDown && moveVec == Vector3.zero && timeSinceAttack > 0.8f)
        {
            if (!isEquipped)
            {
                return;
            }

            currentAttack++;
            isAttack = true;

            // 최대 3콤보
            if (currentAttack > 3)
            {
                currentAttack = 1;
            }

            // 콤보 리셋
            if (timeSinceAttack > 1.0f)
            {
                currentAttack = 1;
            }

            // 콤보에 따른 다른 애니메이션
            anim.SetTrigger("doAttack" + currentAttack);

            // 타이머 리셋
            timeSinceAttack = 0;
        }
    }

    // Attack2 관련 애니메이션에 개입
    public void ResetAttack2()
    {
        isAttack = false;
    }

    void Block()
    {
        if (bDown && moveVec == Vector3.zero)
        {
            anim.SetBool("isBlock", true);
            isBlock = true;
        }
        else
        {
            anim.SetBool("isBlock", false);
            isBlock = false;
        }
    }

    void Dodge()
    {
        if (dDown && !isDodge && moveVec != Vector3.zero)
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
        speed *= 0.5f;
        isDodge = false;
    }
}

