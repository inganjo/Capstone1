using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameClear : MonoBehaviourPunCallbacks
{
    public Text interactionUIText;
    public KeyCode interactKey = KeyCode.E;
    public float teleportCooldown = 0.5f; // 순간이동 쿨다운 추가
    
    private bool isPlayerNear = false;
    private Transform playerTransform;
    private CharacterController playerController; // CharacterController 참조 추가
    private bool canTeleport = true; // 텔레포트 가능 여부
    private float cooldownTimer = 0f;
    private TimerManager timerManager;


    void Start()
    {
        interactionUIText.gameObject.SetActive(false);
        // TimerManager 찾기
        timerManager = FindObjectOfType<TimerManager>();
        if (timerManager == null)
        {
            Debug.LogError("TimerManager를 찾을 수 없습니다.");
        }
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
        if (isPlayerNear && canTeleport && Input.GetKeyDown(interactKey))
        {
            if (timerManager != null)
            {
                timerManager.TriggerGameClear();
                interactionUIText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isPlayerNear = true;
            playerTransform = other.transform;
            playerController = other.GetComponent<CharacterController>(); // CharacterController 가져오기
            interactionUIText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isPlayerNear = false;
            interactionUIText.gameObject.SetActive(false);
        }
    }
}