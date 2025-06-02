using System.Collections.Generic;
using UnityEngine;

public class BTAgent_Defensive : BTAgentBase
{
    private CharacterAction character;

    void Start()
    {
        character = GetComponent<CharacterAction>();
        character.EquipWeapon();


        // ─────────────────── 조건 노드 ───────────────────
        var isLowHP = new IsLowHealth(character, 30f);
        var canAttack = new CanAttack(character);
        var canDefend = new CanDefend(character);
        var canDodge = new CanDodge(character);
        var isEnemyClose = new IsEnemyInRange(character, character.transform, enemy, 1.8f);
        var isEnemyNotFar = new IsEnemyInRange(character, character.transform, enemy, 6f);

        // ─────────────────── 행동 노드 ───────────────────
        var defend = new DefendAction(character, enemy);
        var dodge = new DodgeAction(character, character.transform, enemy);
        var moveAway = new MoveAwayFromEnemyAction(character, character.transform, enemy);
        var counterAttack = new AttackAction(character,enemy);

        // ─────────────────── 시퀀스 노드 ───────────────────
        var defendSeq = new Sequence(new List<Node> { isEnemyClose, canDefend, defend });
        var dodgeSeq = new Sequence(new List<Node> { isEnemyClose, canDodge, dodge });
        var counterSeq = new Sequence(new List<Node> { isEnemyClose, canAttack, counterAttack });
        var moveAwaySeq = new Sequence(new List<Node> { isLowHP, isEnemyNotFar, moveAway });

        // ─────────────────── 최종 트리 구성 ───────────────────
        root = new Selector(new List<Node> {
            moveAwaySeq,   // 가까울 때만 도망
            defendSeq,
            dodgeSeq,
            counterSeq,    
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