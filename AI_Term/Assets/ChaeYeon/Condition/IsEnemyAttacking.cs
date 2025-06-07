using UnityEngine;
public class IsEnemyAttacking : Node
{
    private CharacterAction enemy;

    public IsEnemyAttacking(Transform enemyTransform)
    {
        enemy = enemyTransform.GetComponent<CharacterAction>();
    }

    public override NodeState Evaluate()
    {
        return enemy != null && enemy.IsAttacking() ? NodeState.Success : NodeState.Failure;
    }
}