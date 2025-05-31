using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    [SerializeField]
    GameObject sword;
    [SerializeField]
    GameObject swordOnShoulder;

    public float speed;

    float hAxis;
    float vAxis;
    bool eDown;
    bool aDown;
    bool bDown;
    bool kDown;
    bool dDown;

    bool isEquipping;
    bool isEquipped;
    bool isAttack;
    bool isBlock;
    bool isKick;
    bool isDodge;

    float timeSinceAttack;
    int currentAttack = 0;

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
        Kick();
        Dodge();
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
        // Z key -> 움직이고 있지 않을 때 발차기
        kDown = Input.GetButton("Kick1");
        // Left Shift Key -> 움직이고 있을 때 그 방향으로 회피
        dDown = Input.GetButtonDown("Dodge1");
    }

    void Move()
    {
        if (isEquipping || isBlock || isKick || isAttack)
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

    // Attack1 관련 애니메이션에 개입
    public void ResetAttack1()
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

    void Kick()
    {
        if (kDown && moveVec == Vector3.zero)
        {
            anim.SetBool("isKick", true);
            isKick = true;
        }
        else
        {
            anim.SetBool("isKick", false);
            isKick = false;
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
