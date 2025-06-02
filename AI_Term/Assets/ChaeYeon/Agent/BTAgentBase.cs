using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BTAgentBase : MonoBehaviour
{
    protected AgentStatus agentStatus;
    protected Animator animator;
    public Transform enemy;

    protected Node root;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agentStatus = GetComponent<AgentStatus>();
    }

    protected virtual void Update()
    {
        root?.Evaluate();
    }

    public virtual void OnDeath()
    {
        this.enabled = false;
    }
}
