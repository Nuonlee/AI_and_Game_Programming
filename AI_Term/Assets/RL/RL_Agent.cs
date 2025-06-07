using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.IO;
using System.Reflection;


public class RL_Agent : Agent
{
    
    public RL_Agent opponentAgent;
    public Rigidbody rb;
    public CharacterAction character;
    public Transform StartLocation;

    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldownTime = 2f;  // 공격 쿨타임
    public float defendCooldownTime = 2f;  // 방어 쿨타임
    public float dodgeCooldownTime = 5f;   // 회피 쿨타임
    private float lastAttackTime = 0f;  // 마지막 공격 시각
    private float lastDefendTime = 0f;  // 마지막 방어 시각
    private float lastDodgeTime = 0f;   // 마지막 회피 시각
    public bool alive = true;

    public enum AgentRole { Defender, Attacker }
    public AgentRole role = AgentRole.Attacker;

    private bool rewardGiven = false;
    private static int episodeCount = 0;
    private float episodeStartTime;
    private int episodeStepCount = 0;
    private float totalReward = 0f;
    private static string logDir = "Assets/Logs";
    private static string logFilePath = Path.Combine(logDir, "episode_log.csv");
    private static bool headerWritten = false;
    private float prevOpponentHealth;  // 이전 프레임의 상대 체력
    private Vector3 prevPosition;

    private float equipStartTime;
    private float timeSinceLastAttackAttempt = 0f;
    private float farDistanceDuration = 0f;



    private float lastDistance = 0f;
    public float maxEpisodeTime = 60f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        character = GetComponent<CharacterAction>();
    }

    public override void OnEpisodeBegin()
    {
        episodeCount++;
        episodeStartTime = Time.time;
        episodeStepCount = 0;
        totalReward = 0f;
        rewardGiven = false;
        alive = true;

        prevPosition = transform.position;
        transform.localPosition = StartLocation.position;
        rb.velocity = Vector3.zero;

        character.agentStatus.currentHealth = character.agentStatus.maxHealth;
        character.agentStatus.isDead = false;
        character.agentStatus.isDefending = false;
        character.agentStatus.isBeingAttacked = false;
        character.agentStatus.isInvincible = false;

        character.StopAllCoroutines();
        character.ResetBlockStatus();
        character.enabled = true;
        prevOpponentHealth = opponentAgent.character.agentStatus.currentHealth;


        timeSinceLastAttackAttempt = 0f;
        farDistanceDuration = 0f;



        if (!character.IsEquipped())
        {
            character.EquipWeapon();
            equipStartTime = Time.time;  // ✅ 장착 시각 기록
        }

        // ✅ 애니메이터 상태 초기화
        Animator anim = character.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Rebind();   // 트랜지션 초기화
            anim.Update(0f);
            anim.Play("Idle");  // Idle 애니메이션 이름 확인 후 수정
        }
        if (opponentAgent != null)
        {
            opponentAgent.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            opponentAgent.rb.velocity = Vector3.zero;
            opponentAgent.rewardGiven = false;

            var oppChar = opponentAgent.character;
            oppChar.agentStatus.currentHealth = oppChar.agentStatus.maxHealth;
            oppChar.agentStatus.isDead = false;
            oppChar.agentStatus.isDefending = false;
            oppChar.agentStatus.isBeingAttacked = false;
            oppChar.agentStatus.isInvincible = false;

            oppChar.StopAllCoroutines();
            oppChar.ResetBlockStatus();
            oppChar.enabled = true;
            Animator oppAnim = opponentAgent.character.GetComponentInChildren<Animator>();
            if (oppAnim != null)
            {
                oppAnim.Rebind();
                oppAnim.Update(0f);
                oppAnim.Play("Idle");
            }
            if (!oppChar.IsEquipped())
                oppChar.EquipWeapon();
        }

        lastDistance = Vector3.Distance(transform.position, opponentAgent.transform.position);

        float x = Academy.Instance.EnvironmentParameters.GetWithDefault("agent_start_x", 0f);
        float z = Academy.Instance.EnvironmentParameters.GetWithDefault("agent_start_z", 0f);
        Vector3 randomStartPos = new Vector3(x, 10, z);
        transform.position = StartLocation.position + randomStartPos;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (opponentAgent == null || opponentAgent.character == null || opponentAgent.character.agentStatus == null)
            return;
        // 본인 위치
        sensor.AddObservation(transform.localPosition);  // 3 float

        // 상대 위치
        sensor.AddObservation(opponentAgent.transform.localPosition);  // 3 float

        // 나-상대 간 방향
        Vector3 dirToOpponent = (opponentAgent.transform.position - transform.position).normalized;
        sensor.AddObservation(dirToOpponent); // 3 float

        // 거리 (정규화)
        float distance = Vector3.Distance(transform.position, opponentAgent.transform.position);
        sensor.AddObservation(distance / 10f); // 1 float

        // 체력 정규화 (0~1)
        float healthNorm = character.agentStatus.currentHealth / 100f;
        float oppHealthNorm = opponentAgent.character.agentStatus.currentHealth / 100f;
        sensor.AddObservation(healthNorm);      // 1 float
        sensor.AddObservation(oppHealthNorm);   // 1 float

        // 현재 상태 (공격, 방어, 회피, 장착 여부)
        sensor.AddObservation(character.IsAttacking() ? 1f : 0f);  // 1 float
        sensor.AddObservation(character.IsBlocking() ? 1f : 0f);   // 1 float
        sensor.AddObservation(character.IsDodging() ? 1f : 0f);    // 1 float
        sensor.AddObservation(character.IsEquipping() ? 1f : 0f);  // 1 float

        // 쿨타임 (정규화된 시간)
        sensor.AddObservation(Mathf.Clamp01((Time.time - lastAttackTime) / attackCooldownTime));  // 1 float
        sensor.AddObservation(Mathf.Clamp01((Time.time - lastDefendTime) / defendCooldownTime));  // 1 float
        sensor.AddObservation(Mathf.Clamp01((Time.time - lastDodgeTime) / dodgeCooldownTime));    // 1 float
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        episodeStepCount++;

        if (!character.IsAlive())
        {
            SetReward(-1f);
            opponentAgent.SetReward(opponentAgent.role == AgentRole.Attacker ? +1f : +0.2f);

            totalReward = GetCumulativeReward();
            rewardGiven = true;
            opponentAgent.rewardGiven = true;

            LogEpisode();
            EndEpisode();
            opponentAgent.EndEpisode();
            return;
        }
        float distance = Vector3.Distance(transform.position, opponentAgent.transform.position);

        // ✅ 점진적 거리 보상 (Attacker는 가까울수록, Defender는 멀수록 보상)
        if (role == AgentRole.Attacker)
        {
            float closeReward = Mathf.Clamp01(1f - distance / 5f);
            AddReward(closeReward * 2.0f);  // 최대 0.5f 보상
        }
        else if (role == AgentRole.Defender)
        {
            float farReward = Mathf.Clamp01(distance / 5f);
            AddReward(farReward * 0.2f);  // 최대 0.2f 보상
        }

        // ✅ 체력 감소 확인 (공격 성공 보상)
        float currentOpponentHealth = opponentAgent.character.agentStatus.currentHealth;
        if (currentOpponentHealth < prevOpponentHealth)
        {
            if(role == AgentRole.Attacker)
                AddReward(5f);
        }
        prevOpponentHealth = currentOpponentHealth;

        float delta = lastDistance - distance;

        // 최소 변화폭 필터 제거 (보다 적극적인 보상 유도)
        if (role == AgentRole.Attacker)
        {
            // 가까워질수록 큰 보상 (속도감 강조)
            float scaledDelta = Mathf.Clamp(delta * 3.0f, -1.5f, 1f);  // 적당한 보정
            AddReward(scaledDelta);

            // 현재 절대 거리 기반 보상 추가
            float closeReward = Mathf.Clamp01(1.0f - (distance / 10.0f));  // 0~1
            AddReward(closeReward * 1.5f);
        }
        else if (role == AgentRole.Defender)
        {
            // 가까워지면 위험하므로 멀어지면 보상
            float scaledDelta = Mathf.Clamp(-delta * 1.0f, -0.5f, 0.5f);
            AddReward(scaledDelta);
        }

        lastDistance = distance;


        // ✅ 공격 쿨타임 및 보상
        if (Time.time - lastAttackTime >= attackCooldownTime)
        {
            if (character.TryAttack())
            {
                lastAttackTime = Time.time;
                timeSinceLastAttackAttempt = 0f;
                AddReward(+1.0f);  // 공격 시도에 대한 소형 보상
            }
        }

        // ✅ 방어 쿨타임 및 보상
        if (Time.time - lastDefendTime >= defendCooldownTime)
        {
            if (character.TryDefend())
            {
                lastDefendTime = Time.time;
                AddReward(role == AgentRole.Defender ? +2f : +0.1f);
            }
        }

        // ✅ 회피 쿨타임 및 보상
        if (Time.time - lastDodgeTime >= dodgeCooldownTime && role == AgentRole.Defender)
        {
            Vector3 dodgeDir = transform.position - opponentAgent.transform.position;
            if (character.TryDodge(dodgeDir))
            {
                lastDodgeTime = Time.time;
                AddReward(+0.5f);
            }
        }

        // ✅ 시간 초과 시 종료
        if (Time.time - episodeStartTime >= maxEpisodeTime)
        {
            SetReward(role == AgentRole.Defender ? +1.0f : -0.5f);
            EndEpisodeWithLog();
            return;
        }

        // ✅ 사망 또는 낙하 시 종료
        if (!rewardGiven && (!character.IsAlive() || !opponentAgent.character.IsAlive()) || this.transform.position.y < -5)
        {
            bool selfDied = !character.IsAlive();
            if (selfDied)
            {
                SetReward(-50f);        // 상대는 역할에 따라 보상
                if (opponentAgent.role == AgentRole.Attacker)
                {
                    opponentAgent.SetReward(+25f);  // 공격 성공
                }
                else
                {
                    opponentAgent.SetReward(+5f);   // 방어 잘함
                }
                Debug.Log(this.name + " Died");
            }
            else
            {
                SetReward(role == AgentRole.Attacker ? +25f : +5f);  // 나의 공격 성공
                opponentAgent.SetReward(-30f);  // 죽은 쪽은 패널티
            }
            if (this.transform.position.y < -5)
            {
                SetReward(-1000f);
            }

            EndEpisodeWithLog();
            return;
        }

        // ✅ 이동 처리
        int move = actions.DiscreteActions[0];
        int act = actions.DiscreteActions[1];

        if (move == 1)  // MoveForward
        {
            AddReward(0.2f); // 시도 자체 보상
        }

        Vector3 dir = Vector3.zero;
        switch (move)
        {
            case 1: dir = Vector3.forward; break;
            case 2: dir = Vector3.back; break;
            case 3: dir = Vector3.left; break;
            case 4: dir = Vector3.right; break;
        }
        character.Move(dir);

        // ✅ 행동 처리
        // 공격 및 방어 처리
        switch (act)
        {
            case 1: // 공격
                if (distance < 3.0f)  // 공격 범위 체크
                {
                    character.TurnTo(opponentAgent.transform.position);
                    if (character.TryAttack())
                    {
                        AddReward(+1.0f);
                        timeSinceLastAttackAttempt = 0f;
                    }
                }
                else
                {
                    AddReward(-0.2f); // 의미 없는 공격 시도에 페널티 (선택)
                }
                break;

            case 2: // 방어
                if (distance < 3.0f)  // 공격 받을 수 있는 거리에서만 방어
                {
                    if (character.TryDefend())
                    {
                        AddReward(role == AgentRole.Defender ? +2.0f : +0.1f);
                    }
                }
                else
                {
                    AddReward(-0.1f); // 불필요한 방어 시도는 감점
                }
                break;
        }


        // ✅ 상태 강제 초기화
        if (character.IsAttacking() && Time.time - lastAttackTime > 1.0f)
            character.isAttacking = false;

        if (character.IsBlocking() && Time.time - lastDefendTime > 0.6f)
            character.ResetDefend();

        if (character.IsDodging() && Time.time - lastDodgeTime > 0.6f)
        {
            var f = typeof(CharacterAction).GetField("isDodging", BindingFlags.NonPublic | BindingFlags.Instance);
            f?.SetValue(character, false);
        }

        if (character.IsEquipping() && Time.time - equipStartTime > 1.0f)
        {
            var f = typeof(CharacterAction).GetField("isEquipping", BindingFlags.NonPublic | BindingFlags.Instance);
            f?.SetValue(character, false);
        }
    }

    // ✅ 종료 기록 메서드
    private void EndEpisodeWithLog()
    {
        totalReward = GetCumulativeReward();
        rewardGiven = true;
        opponentAgent.rewardGiven = true;
        LogEpisode();
        EndEpisode();
        opponentAgent.EndEpisode();
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
    }

    private void LogEpisode()
    {
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);

        if (!headerWritten && !File.Exists(logFilePath))
        {
            File.AppendAllText(logFilePath, "AgentName,Episode,StepCount,Reward,EndTime,Duration,IsDead\n");
            headerWritten = true;
        }

        string logLine = $"{gameObject.name},{episodeCount},{episodeStepCount},{totalReward:F2},{Time.time:F2},{Time.time - episodeStartTime:F2},{!character.IsAlive()}\n";
        File.AppendAllText(logFilePath, logLine);
    }
}
