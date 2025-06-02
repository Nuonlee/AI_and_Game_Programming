using UnityEngine;
public class DodgeAction : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform enemy;

    public DodgeAction(CharacterAction character, Transform self, Transform enemy)
    {
        this.character = character;
        this.self = self;
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (character == null || enemy == null || !character.IsAlive())
            return NodeState.Failure;

        // 방향 계산
        Vector3 toEnemy = (enemy.position - self.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, toEnemy).normalized;
        Vector3 dodgeDir = Random.value < 0.5f ? right : -right;

        // 구르기 시도
        bool success = character.TryDodge(dodgeDir);

        if (success)
        {
            Debug.Log("💨 DodgeAction: 회피 성공");
            return NodeState.Success;
        }
        else
        {
            Debug.Log("❌ DodgeAction: 회피 실패");
            return NodeState.Failure;
        }
    }
}