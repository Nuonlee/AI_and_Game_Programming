using UnityEngine;

public class MoveAwayFromEnemyAction : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform enemy;
    private float escapeDistance;

    public MoveAwayFromEnemyAction(CharacterAction character, Transform self, Transform enemy, float escapeDistance = 4f)
    {
        this.character = character;
        this.self = self;
        this.enemy = enemy;
        this.escapeDistance = escapeDistance;
    }

    public override NodeState Evaluate()
    {
        if (enemy == null || character == null || !character.IsAlive())
            return NodeState.Failure;

        float distance = Vector3.Distance(self.position, enemy.position);

        Vector3 awayDir = (self.position - enemy.position).normalized;
        character.Move(awayDir);

        if (distance >= escapeDistance)
        {
            character.StopMove();
            return NodeState.Success;
        }


        return NodeState.Running;
    }
}