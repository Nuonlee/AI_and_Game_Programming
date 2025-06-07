using UnityEngine;
using System.Collections;
public class WasBlockSuccessful : Node
{
    private CharacterAction character;

    public WasBlockSuccessful(CharacterAction character)
    {
        this.character = character;
    }

    public override NodeState Evaluate()
    {
        return character.DidBlockSuccessfully ? NodeState.Success : NodeState.Failure;
    }
}