using UnityEngine;
using System.Collections.Generic;

public class BTAgent_Attack : BTAgentBase
{
    private CharacterAction character;

    void Start()
    {
        character = GetComponent<CharacterAction>();
        character.EquipWeapon();
        float attackRange = 1.75f;
        float chaseRange = 1.7f;
        float dashRange = 5f;

        // ─────────────────── 조건 노드 ───────────────────
        var isLowHP = new IsLowHealth(character, 30f);
        var isEnemyInAttackRange = new IsEnemyInRange(character, character.transform, enemy, attackRange);
        var canAttack = new CanAttack(character);
        var canDefend = new CanDefend(character);
        var canDodge = new CanDodge(character);
        var isEnemyFar = new IsEnemyOutOfRange(character, character.transform, enemy, chaseRange);
        var isEnemyTooFar = new IsEnemyOutOfRange(character, character.transform, enemy, dashRange);

        // ─────────────────── 행동 노드 ───────────────────
        var defend = new DefendAction(character, enemy);
        var attack = new AttackAction(character, enemy);
        var moveToEnemy = new MoveToEnemy(character, character.transform, enemy, attackRange);
        var dashToPlayer = new DashAction(character, character.transform, enemy, 7f);
        var dodge = new DodgeAction(character, character.transform, enemy);

        // ─────────────────── 시퀀스 노드 ───────────────────
        var attackSeq = new Sequence(new List<Node> { isEnemyInAttackRange, canAttack, attack });
        var chaseSeq = new Sequence(new List<Node> { isEnemyFar, moveToEnemy });
        var dashSeq = new Sequence(new List<Node> { isEnemyTooFar, dashToPlayer });
        var defendSeq = new Sequence(new List<Node> { isLowHP, canDefend, defend });
        var dodgeSeq = new Sequence(new List<Node> { isLowHP, canDodge, dodge });

        // ─────────────────── 트리 구성 ───────────────────

        root = new Sequence(new List<Node> {
            new IsEnemyAlive(enemy), 
            new Selector(new List<Node> {
                attackSeq,
                chaseSeq,
                defendSeq,
                dodgeSeq,
            })
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