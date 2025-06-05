using UnityEngine;

public class MoveToEnemy : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform enemy;
    private float stopDistance;

    public MoveToEnemy(CharacterAction character, Transform self, Transform enemy, float stopDistance = 2f)
    {
        this.character = character;
        this.self = self;
        this.enemy = enemy;
        this.stopDistance = stopDistance;
    }

    public override NodeState Evaluate()
    {
        if (enemy == null || character == null || !character.IsAlive())
            return NodeState.Failure;

        float distance = Vector3.Distance(self.position, enemy.position);

        Vector3 direction = (enemy.position - self.position).normalized;
        character.Move(direction);

        if (distance <= stopDistance)
        {
            character.StopMove();
            return NodeState.Success;
        }
        return NodeState.Running;
    }
}