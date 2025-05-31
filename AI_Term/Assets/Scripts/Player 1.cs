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
        // R key -> �����̰� ���� ���� �� �� ����
        eDown = Input.GetButtonDown("Equip1");
        // P key -> �����̰� ���� �ʰ� �� ���� �� ����
        aDown = Input.GetButtonDown("Attack1");
        // E key -> �����̰� ���� ���� �� ���� ����
        bDown = Input.GetButton("Block1");
        // Z key -> �����̰� ���� ���� �� ������
        kDown = Input.GetButton("Kick1");
        // Left Shift Key -> �����̰� ���� �� �� �������� ȸ��
        dDown = Input.GetButtonDown("Dodge1");
    }

    void Move()
    {
        if (isEquipping || isBlock || isKick || isAttack)
        {
            return;
        }

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // ȸ�� ���� ������ �ٲ��� �ʰ� ��
        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * Time.deltaTime;
        anim.SetBool("isMove", moveVec != Vector3.zero);
    }

    // �����̴� �������� �ٶ�
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

    // Equip 1 �ִϸ��̼ǿ� ����
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

            // �ִ� 3�޺�
            if (currentAttack > 3)
            {
                currentAttack = 1;
            }

            // �޺� ����
            if (timeSinceAttack > 1.0f)
            {
                currentAttack = 1;
            }

            // �޺��� ���� �ٸ� �ִϸ��̼�
            anim.SetTrigger("doAttack" + currentAttack);

            // Ÿ�̸� ����
            timeSinceAttack = 0;
        }
    }

    // Attack1 ���� �ִϸ��̼ǿ� ����
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

            // 0.5�� �Ŀ� ȸ�� ����
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
}
