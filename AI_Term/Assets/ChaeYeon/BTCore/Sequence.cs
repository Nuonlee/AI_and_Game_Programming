using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> children;

    public Sequence(List<Node> children)
    {
        this.children = children;
    }

    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result == NodeState.Failure)
                return NodeState.Failure;
            if (result == NodeState.Running)
                return NodeState.Running;
        }

        return NodeState.Success;
    }
}