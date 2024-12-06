using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class TimerManager : MonoBehaviourPunCallbacks
{
    public static TimerManager instance;

    public float timeLimit = 180f;
    private float remainingTime;
    private Text timerText;
    private bool gameOverTriggered = false;
    

    public GameObject gameOverPanel;
    public Text gameOverText;
    private CanvasGroup canvasGroup;

    private PhotonView photonView;
    private PhotonView myphotonView;

    private float startTime;
    
    // 플레이어 참조를 위한 변수 추가
    private GameObject player;

    private GameObject spawnpoint;

    void Awake()
    {
        // photonView = GetComponent<PhotonView>();
        // Debug.Log($"PhotonView 생성됨: {photonView.ViewID}, {gameObject.name}");
        // if (FindObjectsOfType<TimerManager>().Length > 1)
        // {
        //     Debug.LogWarning("중복된 TimerManager 객체 제거");
        //     Destroy(gameObject);
        //     return;
        // }
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
            InitializeGameOverUI();
        }
        else if(instance != null || instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        // photonView = GetComponent<PhotonView>();
        // if (photonView != null && photonView.ViewID == 0)
        // {
        //     Debug.Log($"TimerManager PhotonView 동적 ID 할당: {gameObject.name}");
        //     photonView.ViewID = PhotonNetwork.AllocateViewID(false);
        // }
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
        photonView = GetComponent<PhotonView>();
        Debug.Log("TimeManager 시작됨");
        
            remainingTime = timeLimit;


        
        UpdateTimerUIReference();
        if(PhotonNetwork.IsConnected == false || PhotonNetwork.IsMasterClient){
                FindPlayer();
                FindPlayer();
        }
        MoveSpawnPoint();
        SceneManager.sceneLoaded += OnSceneLoaded;
        startTime = Time.time;
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

        if (gameOverTriggered)
        {
            if(myphotonView.IsMine){
                TriggerGameOver();
            }
        }
    }

    void FindPlayer()
    {
        Debug.Log("FindPlayer 작동");
        if(PhotonNetwork.IsConnected == false){
            Debug.Log("오프라인 view 정상 작동");
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else{
            foreach(GameObject player1 in GameObject.FindGameObjectsWithTag("Player"))
            {
                Debug.Log("player1" + player1);
                PhotonView view = player1.GetComponent<PhotonView>();
                if(view != null && view.IsMine)
                {
                    Debug.Log("view 정상 작동");
                    player = player1;
                    myphotonView = view;
                    break;
                }
            }
        }

        if (player == null)
        {
            Debug.LogWarning("Player를 찾을 수 없습니다.");
        }
    }

    void MoveSpawnPoint(){
        spawnpoint = GameObject.FindGameObjectWithTag("SPAWNPOINT");
        Debug.Log("TimeManager:" +spawnpoint);
        Debug.Log("TimeManager:" +player);
        player.transform.position = spawnpoint.transform.position;
    }

    void Update()
    {
        // Debug.Log("남은 시간" + remainingTime);
        if (remainingTime > 0 && !gameOverTriggered)
        {
            // Debug.Log("Update if문 작동중");
            // if(PhotonNetwork.IsMasterClient){
                remainingTime = timeLimit - (Time.time - startTime);
                // myphotonView.RPC("SyncremainingTime",RpcTarget.All,remainingTime);
            // }

            UpdateTimerUI();

            if (remainingTime <= 0)
            {

                    TriggerGameOver();
        
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Debug.Log("UpdateTimerUI 시작됨");

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
    #region RPC
    [PunRPC]
    void SyncStartTime(float timelimit)
    {
       Debug.Log("SyncStartTime 시작됨");
       remainingTime = timelimit;
    }
    [PunRPC]
    void RPC_TriggerGameOver()
    {
        TriggerGameOver();
    }
    [PunRPC]
    void SyncremainingTime(float currenttime)
    {
        remainingTime = currenttime;
    }
    #endregion


    #region PUN
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + "님이 입장하셨습니다.");

        FindPlayer();


    }
    #endregion
}
