using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public Text interactionUIText;
    public Vector3 teleportPosition;
    public KeyCode interactKey = KeyCode.E;
    public float teleportCooldown = 0.5f; // 순간이동 쿨다운 추가
    
    private bool isPlayerNear = false;
    private Transform playerTransform;
    private CharacterController playerController; // CharacterController 참조 추가
    private bool canTeleport = true; // 텔레포트 가능 여부
    private float cooldownTimer = 0f;

    void Start()
    {
        interactionUIText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 쿨다운 타이머 업데이트
        if (!canTeleport)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= teleportCooldown)
            {
                canTeleport = true;
                cooldownTimer = 0f;
            }
        }

        // 텔레포트 조건 체크
        if (isPlayerNear && canTeleport && Input.GetKeyDown(interactKey))
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerTransform = other.transform;
            playerController = other.GetComponent<CharacterController>(); // CharacterController 가져오기
            interactionUIText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionUIText.gameObject.SetActive(false);
        }
    }

    void TeleportPlayer()
    {
        if (playerController != null)
        {
            // CharacterController를 잠시 비활성화
            playerController.enabled = false;
            // 위치 이동
            playerTransform.position = teleportPosition;
            // CharacterController를 다시 활성화
            playerController.enabled = true;
        }
        else
        {
            // CharacterController가 없는 경우 직접 위치 이동
            playerTransform.position = teleportPosition;
        }

        // 텔레포트 쿨다운 설정
        canTeleport = false;
        cooldownTimer = 0f;
        
        // 트리거 영역에서 벗어나도록 강제로 설정
        isPlayerNear = false;
        interactionUIText.gameObject.SetActive(false);
    }
}