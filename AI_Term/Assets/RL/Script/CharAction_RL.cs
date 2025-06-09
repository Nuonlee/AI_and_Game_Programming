using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAction_RL : CharacterAction
{
    public bool isResetting = false;

    public new void Move(Vector3 direction)
    {
        if (isResetting) return;  // 이동 막기
        base.Move(direction);
    }

    public new bool TryDodge(Vector3 direction)
    {
        if (isResetting) return false;
        return base.TryDodge(direction);
    }

    public new void DashTo(Vector3 targetPos, float duration = 0.5f)
    {
        if (isResetting) return;
        base.DashTo(targetPos, duration);
    }
}
