using UnityEngine;
using System.Collections.Generic;

public class BTAgent_Attack : BTAgentBase
{
    private CharacterAction character;

    void Start()
    {
        character = GetComponent<CharacterAction>();
        character.EquipWeapon();
        float attackRange = 1.8f;

        // ─────────────────── 조건 노드 ───────────────────
        var isLowHP = new IsLowHealth(character, 30f);
        var isEnemyInAttackRange = new IsEnemyInRange(character, character.transform, enemy, attackRange);
        var canAttack = new CanAttack(character);
        var canDodge = new CanDodge(character);
        var isEnemyFar = new IsEnemyOutOfRange(character, character.transform, enemy, attackRange);

        // ─────────────────── 행동 노드 ───────────────────
        var attack = new AttackAction(character, enemy);
        var dodge = new DodgeAction(character, character.transform, enemy);
        var moveToEnemy = new MoveToEnemy(character, character.transform, enemy, attackRange);
        var dashToPlayer = new DashAction(character, character.transform, enemy, 5f);
        var wait = new WaitAction(0.5f);  // 공격이 안 되거나 멈췄을 때 살짝 쉼

        // ─────────────────── 시퀀스 노드 ───────────────────
        var dodgeSeq = new Sequence(new List<Node> { isLowHP, canDodge, dodge });
        var attackSeq = new Sequence(new List<Node> { isEnemyInAttackRange, canAttack, attack });
        var chaseSeq = new Sequence(new List<Node> { isEnemyFar, moveToEnemy });
        var dashSeq = new Sequence(new List<Node> { isEnemyFar, dashToPlayer });

        // ─────────────────── 트리 구성 ───────────────────
        root = new Selector(new List<Node> {
            attackSeq,
            dashSeq,
            chaseSeq,
           // dodgeSeq,
         //   wait
        });

        Debug.Log("[BTAgent_Attack] 트리 구성 완료");
    }

    public override void OnDeath()
    {
        this.enabled = false;
    }

    void Update()
    {
        root?.Evaluate();
    }
}