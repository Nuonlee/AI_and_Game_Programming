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

        Debug.Log("[StopMoveAction] 이동 중지 요청됨");
        return NodeState.Success;
    }
}