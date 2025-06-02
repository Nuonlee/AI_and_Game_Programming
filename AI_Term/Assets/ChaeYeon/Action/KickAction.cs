using UnityEngine;

public class KickAction : Node
{
    private CharacterAction character;

    public KickAction(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        if (!character.IsAlive())
            return NodeState.Failure;

        character.Kick(true);
        return NodeState.Running;
    }
}