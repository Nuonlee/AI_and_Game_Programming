using UnityEngine;

public class CanDefend : Node
{
    private CharacterAction character;

    public CanDefend(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        bool can = character != null
                && character.agentStatus != null
                && character.agentStatus.CanDefend();

        // Debug.Log($"[CanDefend] ¡æ {can}");

        return can ? NodeState.Success : NodeState.Failure;
    }
}