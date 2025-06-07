using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsEnemyAlive : Node
{
    private Transform enemyTransform;

    public IsEnemyAlive(Transform enemyTransform)
    {
        this.enemyTransform = enemyTransform;
    }

    public override NodeState Evaluate()
    {
        if (enemyTransform == null)
        {
            Debug.Log("❌ [BT] Enemy Transform is null");
            return NodeState.Failure;
        }

        AgentStatus status = enemyTransform.GetComponent<AgentStatus>();
        if (status == null || status.isDead || status.currentHealth <= 0f)
        {
            Debug.Log("💀 [BT] Enemy is dead");
            return NodeState.Failure;
        }

        return NodeState.Success;
    }
}