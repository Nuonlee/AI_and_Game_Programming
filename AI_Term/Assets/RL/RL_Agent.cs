using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class RL_Agent : Agent
{
    public RL_Agent opponentAgent;           // 상대 에이전트
    public Rigidbody rb;
    public CharacterAction character;

    public float moveSpeed = 2f;
    public float attackRange = 2f;

    public enum AgentRole { Defender, Attacker }
    public AgentRole role = AgentRole.Attacker; // 기본은 공격형

    private bool rewardGiven = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        character = GetComponent<CharacterAction>();
    }

    public override void OnEpisodeBegin()
    {
        // 내 위치 초기화
        this.transform.localPosition = new Vector3(-2f, 0.5f, 0);
        rb.velocity = Vector3.zero;
        character.agentStatus.ResetStatus();
        rewardGiven = false;

        // 상대 위치 초기화
        if (opponentAgent != null)
        {
            opponentAgent.transform.localPosition = new Vector3(2f, 0.5f, 0);
            opponentAgent.rb.velocity = Vector3.zero;
            opponentAgent.character.agentStatus.ResetStatus();
            opponentAgent.rewardGiven = false;
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (opponentAgent == null || opponentAgent.character == null || opponentAgent.character.agentStatus == null)
            return;

        // 자기 위치 및 속도
        sensor.AddObservation(transform.localPosition);     // (3)
        sensor.AddObservation(rb.velocity);                 // (3)

        // 상대 위치 및 거리
        Vector3 relPos = opponentAgent.transform.localPosition - transform.localPosition;
        sensor.AddObservation(relPos.normalized);           // (3)
        sensor.AddObservation(relPos.magnitude);            // (1)

        // 내 체력
        sensor.AddObservation(character.agentStatus.currentHealth); // (1)
        // 상대 체력
        sensor.AddObservation(opponentAgent.character.agentStatus.currentHealth); // (1)
        // 총 12차원
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (opponentAgent == null || opponentAgent.character == null || opponentAgent.character.agentStatus == null)
            return;

        // 상대 죽음 감지
        if (!opponentAgent.character.IsAlive() && !rewardGiven)
        {
            if (role == AgentRole.Attacker)
                SetReward(+1.0f); // 공격형이 적 처치 시 큰 보상
            else
                SetReward(+0.2f); // 수비형도 약간의 보상

            rewardGiven = true;
            EndEpisode();

            opponentAgent.SetReward(-1f);
            opponentAgent.rewardGiven = true;
            opponentAgent.EndEpisode();
            return;
        }

        int move = actions.DiscreteActions[0];
        int act = actions.DiscreteActions[1];

        // 이동
        Vector3 dir = Vector3.zero;
        switch (move)
        {
            case 1: dir = Vector3.forward; break;
            case 2: dir = Vector3.back; break;
            case 3: dir = Vector3.left; break;
            case 4: dir = Vector3.right; break;
        }
        character.Move(dir);

        // 행동
        switch (act)
        {
            case 1: // 공격
                character.TurnTo(opponentAgent.transform.position);
                if (character.TryAttack())
                {
                    SetReward(role == AgentRole.Attacker ? +0.6f : +0.1f);
                }
                break;

            case 2: // 방어
                if (character.TryDefend())
                {
                    SetReward(role == AgentRole.Defender ? +0.6f : +0.05f);
                }
                break;

            case 3: // 발차기
                if (character.KickAttack())
                {
                    SetReward(role == AgentRole.Attacker ? +0.8f : +0.2f);
                }
                break;
        }

        if (!character.IsAlive() && !rewardGiven)
        {
            SetReward(-1f);
            rewardGiven = true;
            EndEpisode();

            opponentAgent.SetReward(+1f);
            opponentAgent.rewardGiven = true;
            opponentAgent.EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = 0; d[1] = 0;

        if (Input.GetKey(KeyCode.W)) d[0] = 1;
        if (Input.GetKey(KeyCode.S)) d[0] = 2;
        if (Input.GetKey(KeyCode.A)) d[0] = 3;
        if (Input.GetKey(KeyCode.D)) d[0] = 4;

        if (Input.GetKey(KeyCode.J)) d[1] = 1;
        else if (Input.GetKey(KeyCode.K)) d[1] = 2;
        else if (Input.GetKey(KeyCode.L)) d[1] = 3;
    }
}