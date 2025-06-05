using UnityEngine;
using System.Collections;

public class ResetDefendAction : Node
{
    private CharacterAction character;

    public ResetDefendAction(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        character.DidBlockSuccessfully = false;
        return NodeState.Success;
    }
}