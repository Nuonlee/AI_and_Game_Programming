using UnityEngine;

public class WaitAction : Node
{
    private float duration;
    private float startTime;
    private bool started = false;

    public WaitAction(float seconds)
    {
        duration = seconds;
    }

    public override NodeState Evaluate()
    {
        if (!started)
        {
            startTime = Time.time;
            started = true;
        }

        if (Time.time - startTime >= duration)
        {
            started = false;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}