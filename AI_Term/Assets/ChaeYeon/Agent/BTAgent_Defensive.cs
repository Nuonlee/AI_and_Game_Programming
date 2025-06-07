using System.Collections.Generic;
using UnityEngine;

public class BTAgent_Defensive : BTAgentBase
{
    private CharacterAction character;

    void Start()
    {
        character = GetComponent<CharacterAction>();
        character.EquipWeapon();

        float attackRange = 1.75f;
        // ─────────────────── 조건 노드 ───────────────────
        var isLowHP = new IsLowHealth(character, 30f);
        var canAttack = new CanAttack(character);
        var canDefend = new CanDefend(character);
        var canDodge = new CanDodge(character);
        var isEnemyClose = new IsEnemyInRange(character, character.transform, enemy, attackRange);
        var isEnemyNotFar = new IsEnemyInRange(character, character.transform, enemy, 6f);
        var isEnemyAttacking = new IsEnemyAttacking(enemy); // 적군이 공격중일 경우
        var wasBlockSuccessful = new WasBlockSuccessful(character);

        // ─────────────────── 행동 노드 ───────────────────
        var defend = new DefendAction(character, enemy);
        var dodge = new DodgeAction(character, character.transform, enemy);
        var moveAway = new MoveAwayFromEnemyAction(character, character.transform, enemy);
        var counterAttack = new CounterAttackAction(character, enemy, 0.5f);
        var resetBlockStatus = new ResetDefendAction(character);

        // ─────────────────── 시퀀스 노드 ───────────────────
        var defendSeq = new Sequence(new List<Node> { isEnemyClose, canDefend, isEnemyAttacking, defend }); // 적이 가까움 + Defend 가능 + 적이 공격중일 경우
        var counterSeq = new Sequence(new List<Node> { wasBlockSuccessful, counterAttack, resetBlockStatus }); // 적이 가까움 + 공격 가능할 경우
        var dodgeSeq = new Sequence(new List<Node> { isEnemyClose, canDodge, dodge });  // 적이 가까움 +  Dodge 가능할 경우

        // ─────────────────── 최종 트리 구성 ───────────────────

        root = new Sequence(new List<Node> {
            new IsEnemyAlive(enemy),   // 적이 죽었을 경우 밑의 행동 노드 실행 안 됨
            new Selector(new List<Node> {
                defendSeq,
                counterSeq,
                dodgeSeq,
            })
        });

        Debug.Log("[BTAgent_Defensive] 트리 구성 완료");
    }

    void Update()
    {
        root?.Evaluate();
    }

    public override void OnDeath()
    {
        this.enabled = false;
    }
}