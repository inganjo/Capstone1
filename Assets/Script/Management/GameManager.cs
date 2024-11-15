using Photon.Pun;               // Photon Unity Networking 기능을 사용하기 위해 필요
using Photon.Realtime;           // Photon Realtime 기능을 사용하기 위해 필요
using StarterAssets;
using System.Collections;        // 컬렉션 라이브러리 사용 (리스트 및 딕셔너리)
using System.Collections.Generic; // 컬렉션 사용을 위한 제네릭 리스트, 딕셔너리 라이브러리 사용
using TMPro;
using UnityEngine;               // Unity 엔진 기능 사용
using UnityEngine.SceneManagement;
using UnityEngine.UI;            // Unity UI 기능 사용
using static UnityEngine.EventSystems.PointerEventData;  // 유니티의 이벤트 시스템 사용 (입력 이벤트 관련)

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject UIinventory;


    //ping stat
    private List<int> pingValues = new List<int>();
    private float timeElapsed = 0f;
    private float pingInterval = 1f;

    private bool isSpawn;
    // Start is called before the first frame update

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("GameManager Awake");

    }
    void Start()
    {
        Debug.Log("Start() start");
        // SpawnPlayer();
        
        
    }




    // Update is called once per frame
    void Update()
    {

        timeElapsed += Time.deltaTime;

        // 주기적으로 핑을 측정하고 저장
        if (timeElapsed >= pingInterval)
        {
            int currentPing = PhotonNetwork.GetPing();
            pingValues.Add(currentPing);
            Debug.Log("현재 핑: " + currentPing + " ms");

            timeElapsed = 0f;
        }


        // 네트워크 통계 정보 활성화
        PhotonNetwork.NetworkStatisticsEnabled = true;

// 네트워크 통계 정보 확인
        string stats = PhotonNetwork.NetworkStatisticsToString();
        Debug.Log("네트워크 통계: " + stats);
    }
    void SpawnInventory(){
        Debug.Log("SpawnInventory success");
        Instantiate(UIinventory,new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + "entered the Room");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
    }
    void OnDestroy()
{
    Debug.Log("GameManager destroyed!");// 씬 전환 전 리스너 해제
    OnGUI();
}

//네트워크 측정 정보
    public float GetAveragePing()
    {
        if (pingValues.Count == 0)
            return 0f;

        int sum = 0;
        foreach (int ping in pingValues)
        {
            sum += ping;
        }

        return (float)sum / pingValues.Count;
    }
    void OnGUI()
    {

            float averagePing = GetAveragePing();
            Debug.Log("평균 핑: " + averagePing + " ms");
    
    }
}
//  void Start()
//     {
//         SceneManager.sceneLoaded +=OnSceneLoaded;
        
        
//     }
//     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         if(PhotonNetwork.InRoom)
//         {
//             SpawnPlayer();
//         }
//     }

