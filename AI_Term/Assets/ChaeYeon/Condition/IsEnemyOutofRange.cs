using UnityEngine;

public class IsEnemyOutOfRange : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform enemy;
    private float range;

    public IsEnemyOutOfRange(CharacterAction character, Transform self, Transform enemy, float range)
    {
        this.character = character;
        this.self = self;
        this.enemy = enemy;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        if (character == null || enemy == null || !character.IsAlive())
            return NodeState.Failure;

        float distance = Vector3.Distance(self.position, enemy.position);
        return distance >= range ? NodeState.Success : NodeState.Failure;
    }
}