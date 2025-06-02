using UnityEngine;

public class StopMoveAction : Node
{
    private CharacterAction character;

    public StopMoveAction(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        if (character == null || !character.IsAlive())
            return NodeState.Failure;

        character.StopMove();

        Debug.Log("[StopMoveAction] �̵� ���� ��û��");
        return NodeState.Success;
    }
}