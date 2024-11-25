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

<<<<<<< HEAD
    private PhotonView photonView;

    private float startTime;
    
    // 플레이어 참조를 위한 변수 추가
=======
>>>>>>> d0c2505c27d746cc12d8dd1495920ff8b806321d
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
        FindPlayer();
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
<<<<<<< HEAD
        FindPlayer(); // 씬 로드시 플레이어 다시 찾기
        if(PhotonNetwork.IsConnected ==true && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncStartTime",RpcTarget.All,startTime);
        }
=======
        FindPlayer();

>>>>>>> d0c2505c27d746cc12d8dd1495920ff8b806321d
        if (gameOverTriggered)
        {
            TriggerGameOver();
        }
    }

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

    public void TriggerGameOver()
    {
        gameOverTriggered = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            gameOverPanel.SetActive(true);
            Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>(); // 부모 Canvas 가져오기
            if (canvas != null)
            {
                canvas.sortingOrder = 999; // 높은 우선순위로 설정
            }


            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeInGameOverPanel());
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

        DisablePlayerControl();
    }

    public void TriggerGameClear()
    {
        gameOverTriggered = true;
        gameOverText.text = "Game Clear!";
        gameOverText.color = Color.black;
        Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>(); // 부모 Canvas 가져오기
        if (canvas != null)
        {
            canvas.sortingOrder = 999; // 높은 우선순위로 설정
        }


        if (gameOverPanel != null)
        {
            gameOverPanel.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            gameOverPanel.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeInGameOverPanel());
            }
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        DisablePlayerControl();
    }

    void DisablePlayerControl()
    {
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
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
<<<<<<< HEAD
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
=======
>>>>>>> d0c2505c27d746cc12d8dd1495920ff8b806321d
}
