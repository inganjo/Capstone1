using Photon.Pun;
using UnityEngine;

public class ElevatorGameover : MonoBehaviour
{
    // TimerManager 참조
    private TimerManager timerManager;

    void Start()
    {
        // TimerManager 찾기
        timerManager = FindObjectOfType<TimerManager>();
        if (timerManager == null)
        {
            Debug.LogError("TimerManager를 찾을 수 없습니다.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 범위에 들어왔는지 확인
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            if (timerManager != null)
            {
                timerManager.TriggerGameOver();
            }
        }
    }
}