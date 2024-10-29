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
        // Debug.Log(PhotonNetwork.CountOfPlayers);
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

