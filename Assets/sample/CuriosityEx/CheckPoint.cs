using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// チェックポイント
public class CheckPoint : MonoBehaviour
{
    public RaycastAgent2 agent;
    public int checkPointId;

    // 他のオブジェクトとの交差開始時に呼ばれる
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.agent.EnterCheckPoint(this.checkPointId);
        }
    }
}
