using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public GameObject interactionUIImage; // '문 열기 [F]' 안내 UI 이미지 오브젝트
    public Vector3 teleportPosition;  // 문 뒤의 순간이동 위치 (좌표)
    public KeyCode interactKey = KeyCode.F; // 상호작용 키
    private bool isPlayerNear = false;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = interactionUIImage.GetComponent<SpriteRenderer>();
        // UI 오브젝트를 투명하게 설정
        Color color = spriteRenderer.color;
        color.a = 0; // 투명도 0
        spriteRenderer.color = color; // UI 비활성화
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactKey))
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
            // UI 이미지를 보이게 설정
            Color color = spriteRenderer.color;
            color.a = 1; // 투명도 1로 변경
            spriteRenderer.color = color; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            // UI 이미지를 다시 투명하게 설정
            Color color = spriteRenderer.color;
            color.a = 0; // 투명도 0
            spriteRenderer.color = color; 
        }
    }

    void TeleportPlayer()
    {
        playerTransform.position = teleportPosition; // 문 뒤 좌표로 순간이동
        // 순간이동 후 UI 비활성화
        Color color = spriteRenderer.color;
        color.a = 0; // 투명도 0
        spriteRenderer.color = color; 
    }
}
