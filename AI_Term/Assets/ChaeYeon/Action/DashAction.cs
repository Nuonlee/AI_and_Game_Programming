using UnityEngine;

public class DashAction : Node
{
    private CharacterAction character;
    private Transform self;
    private Transform target;
    private float triggerDistance;

    public DashAction(CharacterAction character, Transform self, Transform target, float triggerDistance = 5f)
    {
        this.character = character;
        this.self = self;
        this.target = target;
        this.triggerDistance = triggerDistance;
    }

    public override NodeState Evaluate()
    {
        if (character == null || target == null || !character.IsAlive())
            return NodeState.Failure;

        float distance = Vector3.Distance(self.position, target.position);

        if (distance > triggerDistance)
        {
            character.DashTo(target.position);
            return NodeState.Success; // 한 번만 대시 시도하고 성공 처리
        }

        return NodeState.Failure;
    }
}