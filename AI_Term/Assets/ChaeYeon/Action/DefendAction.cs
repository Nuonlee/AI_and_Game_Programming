using UnityEngine;

public class DefendAction : Node
{
    private CharacterAction character;
    private Transform enemy;

    public DefendAction(CharacterAction character, Transform enemy)
    {
        this.character = character;
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (character == null || !character.IsAlive())
            return NodeState.Failure;

        if (enemy != null)
            character.TurnTo(enemy.position);

        bool success = character.TryDefend();

        if (success)
        {
            return NodeState.Success;
        }
        else
        {
            return NodeState.Failure;
        }
    }
}