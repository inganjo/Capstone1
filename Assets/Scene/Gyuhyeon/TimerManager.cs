using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerManager : MonoBehaviourPunCallbacks
{
    private static TimerManager instance;

    public float timeLimit = 180f;
    private float remainingTime;
    private Text timerText;
    private bool gameOverTriggered = false;

    public GameObject gameOverPanel;
    public Text gameOverText;
    private CanvasGroup canvasGroup;

    private PhotonView photonView;

    private float startTime;
    
    // 플레이어 참조를 위한 변수 추가
    private GameObject player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            photonView = GetComponent<PhotonView>();
            DontDestroyOnLoad(this.gameObject);
            InitializeGameOverUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameOverUI()
    {
        if (gameOverPanel != null)
        {
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 0f;
            gameOverPanel.SetActive(false);
        }
    }

    void Start()
    {
        if(PhotonNetwork.IsConnected == false){
            remainingTime = timeLimit;
        }
        else{
            if(PhotonNetwork.IsMasterClient)
            {
                startTime = Time.time;
                photonView.RPC("SyncStartTime", RpcTarget.All,startTime);
            }
        }
        UpdateTimerUIReference();
        FindPlayer(); // 플레이어 찾기
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateTimerUIReference();
        InitializeGameOverUI();
        FindPlayer(); // 씬 로드시 플레이어 다시 찾기
        if(PhotonNetwork.IsConnected ==true && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncStartTime",RpcTarget.All,startTime);
        }
        if (gameOverTriggered)
        {
            TriggerGameOver();
        }
    }

    // 플레이어 찾기 함수 추가
    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);
        if (player == null)
        {
            Debug.LogWarning("Player를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (remainingTime > 0 && !gameOverTriggered)
        {
            remainingTime = timeLimit - (Time.time - startTime);
            UpdateTimerUI();

            if (remainingTime <= 0)
            {
                if(PhotonNetwork.IsConnected == false)
                {
                    TriggerGameOver();
                }
                else if(PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("RPC_TriggerGameOver", RpcTarget.All);
                }
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            Debug.Log("작동됨");
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void UpdateTimerUIReference()
    {
        Debug.Log("UpdateTimeUIReference 실행");
        timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
        if (timerText == null)
        {
            Debug.Log("TimerText UI를 찾을 수 없습니다.");
        }
    }

void TriggerGameOver()
    {
        gameOverTriggered = true;

        // UI 처리
        if (gameOverPanel != null)
        {
            // 패널의 Image 컴포넌트 색상 설정 (검정색)
            gameOverPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            
            // 패널 활성화 및 페이드 효과
            gameOverPanel.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeInGameOverPanel());
            }
            else
            {
                Debug.LogError("CanvasGroup이 없습니다!");
            }
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!";
        }

        // 플레이어 제어 비활성화
        DisablePlayerControl();
    }

    // 플레이어 제어 비활성화 함수 추가
    void DisablePlayerControl()
    {
        if (player != null)
        {
            // Rigidbody 처리
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            // Rigidbody2D 처리
            Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // 플레이어 스크립트 비활성화
            // PlayerController라는 이름의 스크립트를 사용하는 경우
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                // PlayerController 또는 이와 유사한 이름의 스크립트를 찾아 비활성화
                if (script.GetType().Name.Contains("Player") || 
                    script.GetType().Name.Contains("Movement") || 
                    script.GetType().Name.Contains("Control"))
                {
                    script.enabled = false;
                }
            }
        }
    }

    IEnumerator FadeInGameOverPanel()
    {
        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            }
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }
    #region RPC
    [PunRPC]
    void SyncStartTime(float masterStartTime)
    {
       startTime = masterStartTime;
    }
    [PunRPC]
    void RPC_TriggerGameOver()
    {
        TriggerGameOver();
    }
    #endregion
}