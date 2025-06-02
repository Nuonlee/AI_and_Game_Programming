using UnityEngine;

public class AttackAction : Node
{
    private CharacterAction character;
    private Transform enemy;

    public AttackAction(CharacterAction character, Transform enemy = null)
    {
        this.character = character;
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (character == null || !character.IsAlive())
            return NodeState.Failure;

        // 공격 전에 적을 바라보도록 회전
        if (enemy != null)
            character.TurnTo(enemy.position);

        bool success = character.TryAttack();

        if (success)
        {
            Debug.Log("▶ AttackAction: 공격 성공");
            return NodeState.Success;
        }
        else
        {
            Debug.Log("❌ AttackAction: 공격 실패");
            return NodeState.Failure;
        }
    }
}