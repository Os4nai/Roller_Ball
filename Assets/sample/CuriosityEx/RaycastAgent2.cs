using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

// RaycastAgent
public class RaycastAgent2 : Agent
{
    Rigidbody rBody;
    int lastCheckPoint; // 最終チェックポイント
    int checkPointCount; // チェックポイント通過数

    // ゲームオブジェクト生成時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
        // 周回数のリセット
        this.lastCheckPoint = 0;
        this.checkPointCount = 0;
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rBody.velocity.x); // RaycastAgentのX速度
        sensor.AddObservation(rBody.velocity.z); // RaycastAgentのZ速度
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // RaycastAgentに力を加える
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = actionBuffers.DiscreteActions[0];
        if (action == 1) dirToGo = transform.forward;
        if (action == 2) dirToGo = transform.forward * -1.0f;
        if (action == 3) rotateDir = transform.up * -1.0f;
        if (action == 4) rotateDir = transform.up;
        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        this.rBody.AddForce(dirToGo * 0.4f, ForceMode.VelocityChange);

        // ステップ毎の報酬
        AddReward(-0.001f);
    }

    // チェックポイントに衝突時に呼ばれる
    public void EnterCheckPoint(int checkPoint)
    {
        // 次のチェックポイントに衝突
        if (checkPoint == (this.lastCheckPoint+1)%4)
        {
            this.checkPointCount++;

            // ゴール
            if (this.checkPointCount >= 4)
            {
                AddReward(2.0f);
                EndEpisode();
            }
        }
        // 前のチェックポイントに衝突
        else if (checkPoint == (this.lastCheckPoint-1+4)%4)
        {
            this.checkPointCount--;
        }

        // 最終チェックポイントの更新
        this.lastCheckPoint = checkPoint;
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        var actionsOut = actionBuffers.DiscreteActions;
        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow)) actionsOut[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) actionsOut[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actionsOut[0] = 3;
        if (Input.GetKey(KeyCode.RightArrow)) actionsOut[0] = 4;
    }
}