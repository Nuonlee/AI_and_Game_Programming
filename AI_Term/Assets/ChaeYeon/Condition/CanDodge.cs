using UnityEngine;

public class CanDodge : Node
{
    private CharacterAction character;

    public CanDodge(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        bool can = character != null
            && character.agentStatus != null
            && character.agentStatus.CanDodge();

        return can ? NodeState.Success : NodeState.Failure;
    }
}