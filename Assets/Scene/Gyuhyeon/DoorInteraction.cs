using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스 추가

public class DoorInteraction : MonoBehaviour
{
    public Text interactionUIText; // '문 열기 [E]' 안내 텍스트 오브젝트
    public Vector3 teleportPosition; // 문 뒤의 순간이동 위치 (좌표)
    public KeyCode interactKey = KeyCode.E; // 상호작용 키
    private bool isPlayerNear = false; // 플레이어가 문 근처에 있는지 여부
    private Transform playerTransform; // 플레이어의 트랜스폼

    void Start()
    {
        // UI 오브젝트를 비활성화
        interactionUIText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 플레이어가 문 근처에 있고 상호작용 키를 눌렀다면
        if (isPlayerNear && Input.GetKeyDown(interactKey))
        {
            TeleportPlayer(); // 플레이어를 순간이동
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 문에 접근했을 경우
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true; // 플레이어가 문 근처에 있음
            playerTransform = other.transform; // 해당 플레이어의 트랜스폼 저장
            interactionUIText.gameObject.SetActive(true); // UI 텍스트 보이기
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 문에서 나갔을 경우
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false; // 플레이어가 문 근처에서 벗어남
            interactionUIText.gameObject.SetActive(false); // UI 텍스트 숨기기
        }
    }

    void TeleportPlayer()
    {
        // 플레이어를 문 뒤의 지정된 위치로 순간이동
        playerTransform.position = teleportPosition; 
    }
}
