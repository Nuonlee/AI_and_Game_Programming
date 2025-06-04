using UnityEngine;

public class CanAttack : Node
{
    private CharacterAction character;

    public CanAttack(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        bool can = character != null && character.agentStatus != null && character.agentStatus.CanAttack();

        //Debug.Log("[CanAttack] Evaluate »£√‚µ ");
        //Debug.Log($"[CanAttack] °Ê {can}");

        return can ? NodeState.Success : NodeState.Failure;
    }
}