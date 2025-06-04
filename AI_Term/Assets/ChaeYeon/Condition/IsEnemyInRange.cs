using UnityEngine;

public class IsEnemyInRange : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform enemy;
    private float range;

    public IsEnemyInRange(CharacterAction character, Transform self, Transform enemy, float range)
    {
        this.character = character;
        this.self = self;
        this.enemy = enemy;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        if (character == null || !character.IsAlive() || enemy == null)
            return NodeState.Failure;

        float distance = Vector3.Distance(self.position, enemy.position);
        return distance < range ? NodeState.Success : NodeState.Failure;
    }
}