using UnityEngine;

public class CounterAttackAction : Node
{
    private CharacterAction character;
    private Transform enemy;
    private float counterChance;

    public CounterAttackAction(CharacterAction character, Transform enemy, float chance = 1.0f)
    {
        this.character = character;
        this.enemy = enemy;
        this.counterChance = Mathf.Clamp01(chance);
    }

    public override NodeState Evaluate()
    {
        if (!character.IsAlive())
            return NodeState.Failure;

        // 공격 전에 적을 바라보도록 회전
        if (enemy != null)
            character.TurnTo(enemy.position);

        // 일정 확률로 반격 시도
        if (Random.value > counterChance)
        {
            Debug.Log("⚠️ CounterAttackAction: 확률 실패로 반격 안 함");
            return NodeState.Failure;
        }

        // 강제 공격 (쿨타임 무시)
        character.CounterAttack();
        return NodeState.Success;
    }
}