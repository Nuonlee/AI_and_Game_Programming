using UnityEngine;

public class IsBeingAttacked : Node
{
    private CharacterAction character;

    public IsBeingAttacked(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        if (character == null || character.agentStatus == null)
            return NodeState.Failure;

        return character.agentStatus.isBeingAttacked
            ? NodeState.Success
            : NodeState.Failure;
    }
}