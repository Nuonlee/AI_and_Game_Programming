using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
    private Node child;

    public Inverter(Node child)
    {
        this.child = child;
    }

    public override NodeState Evaluate()
    {
        NodeState result = child.Evaluate();

        switch (result)
        {
            case NodeState.Success:
                return NodeState.Failure;
            case NodeState.Failure:
                return NodeState.Success;
            case NodeState.Running:
                return NodeState.Running;
            default:
                return NodeState.Failure;
        }
    }
}