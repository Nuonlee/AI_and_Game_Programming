using UnityEngine;

public class IsLowHealth : Node
{
    private CharacterAction character;
    private float threshold;

    public IsLowHealth(CharacterAction character, float threshold)
    {
        this.character = character;
        this.threshold = threshold;
    }

    public override NodeState Evaluate()
    {
        if (character == null || character.agentStatus == null || !character.IsAlive())
            return NodeState.Failure;

        float currentHP = character.agentStatus.currentHealth;
        bool result = currentHP <= threshold;

        return result ? NodeState.Success : NodeState.Failure;
    }
}