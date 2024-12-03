using Photon.Pun;               // Photon Unity Networking 기능을 사용하기 위해 필요
using Photon.Realtime;           // Photon Realtime 기능을 사용하기 위해 필요
using System.Collections;        // 컬렉션 라이브러리 사용 (리스트 및 딕셔너리)
using System.Collections.Generic; // 컬렉션 사용을 위한 제네릭 리스트, 딕셔너리 라이브러리 사용
using TMPro;
using UnityEngine;               // Unity 엔진 기능 사용
using UnityEngine.UI;            // Unity UI 기능 사용
using static UnityEngine.EventSystems.PointerEventData;  // 유니티의 이벤트 시스템 사용 (입력 이벤트 관련)
using UnityEngine.SceneManagement;
using StarterAssets;

public class NetworkManager : MonoBehaviourPunCallbacks // Photon의 MonoBehaviourPunCallbacks 상속 (Photon 관련 콜백을 받기 위해)
{
   [SerializeField] TMP_InputField RoomName;
   [SerializeField] TMP_InputField MaxRoom;
   [SerializeField] Button ConnectRoom;
   [SerializeField] Button CreateRoom;
   [SerializeField] GameObject roomListItem;
   public Transform rtContent;
   public GameObject player;
   Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();


   void Awake(){
      PhotonNetwork.AutomaticallySyncScene = true;
      DontDestroyOnLoad(this.gameObject);
   }

   void Start()
   {
      RoomName.onValueChanged.AddListener(OnNameValueChanged);
      MaxRoom.onValueChanged.AddListener(OnPlayerNumberChange);
      ConnectRoom.onClick.AddListener(OnClickConnectRoom);
      CreateRoom.onClick.AddListener(OnClickCreateRoom);
      SceneManager.sceneLoaded +=OnSceneLoaded;
   }
   void Update()
   {
      // Debug.Log(PhotonNetwork.InLobby);
   }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("UpdateRoomListItem() 호출됨. 방 개수: " + roomList.Count);
        Debug.Log("OnRoomListUpdate() success");
        base.OnRoomListUpdate(roomList);
        DeleteRoomListItem();
        UpdateRoomListItem(roomList);
        CreateRoomListItem();

    }
    void SelectRoomItem(string roomName)
    {
      RoomName.text = roomName;
    }
    void DeleteRoomListItem()
    {
      Debug.Log("DeleteRoomListItem() success");
      foreach(Transform tr in rtContent)
      {
         Destroy(tr.gameObject);
      }
    }
    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
      Debug.Log("UpdateRoomListItem() success");
      foreach( RoomInfo info in roomList)
      {
         if(dicRoomInfo.ContainsKey(info.Name))
         {
            if(info.RemovedFromList)
            {
               dicRoomInfo.Remove(info.Name);
               continue;
            }

         }
         dicRoomInfo[info.Name] = info;
      }
    }

    void CreateRoomListItem()
    {
      Debug.Log("CreateRoomListItem() success");
      foreach(RoomInfo info in dicRoomInfo.Values)
      {
         GameObject go = Instantiate(roomListItem, rtContent);
         RoomListItem item = go.GetComponent<RoomListItem>();
         item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
         item.onDelegate = SelectRoomItem;
         Debug.Log($"프리팹 위치: {go.transform.localPosition}");
      }
    }
    void OnNameValueChanged(string s)
    {
      ConnectRoom.interactable = s.Length >0;
      if(RoomName.text=="")
      {
         CreateRoom.interactable = false;
      }
    }
    void OnPlayerNumberChange(string s)
   {
      CreateRoom.interactable = s.Length>0;
      if(MaxRoom.text =="")
      {
         CreateRoom.interactable = false;
      }
   }

   void OnClickCreateRoom(){ //CreateRoom 이후에 Photon은 자동적으로 방에 Join 시킨다.
      RoomOptions options = new RoomOptions();
      options.MaxPlayers = int.Parse(MaxRoom.text);
      options.IsVisible = true;
      options.IsOpen = true;
      PhotonNetwork.CreateRoom(RoomName.text,options);
   }

    void SpawnPlayer(GameObject other)
    {
        if(PhotonNetwork.LocalPlayer == null || !PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Player not in the room");
            return;
        }
        else{
            if (ThirdPersonController.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(this.player.name,other.transform.position, Quaternion.identity, 0);
            }
           
        }
    }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Active");
        if(PhotonNetwork.InRoom)
        {
            GameObject spawnpoint = GameObject.FindGameObjectWithTag("SPAWNPOINT");
            SpawnPlayer(spawnpoint);
        }
    }
   void OnDestroy()
   {
      Debug.Log("GameManager destroyed!");// 씬 전환 전 리스너 해제
      SceneManager.sceneLoaded -=OnSceneLoaded;
   }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("CreateRoom Failed: "+message);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("CreateRoom() Success");

    }
    
    public void OnClickConnectRoom(){
      PhotonNetwork.JoinRoom(RoomName.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom() success");
        PhotonNetwork.LoadLevel("GH3Floor");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRoomFailed() 작동" + message);
    }
    void JoinOrCreateRoom()
    {
      RoomOptions options = new RoomOptions();
      options.MaxPlayers = int.Parse(MaxRoom.text);
      options.IsVisible = true;
      options.IsOpen = true;
      PhotonNetwork.CreateRoom(RoomName.text,options,TypedLobby.Default);
    }
    void JoinRandomRoom()
    {
      PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("OnJoinedRandomFailed(): " + message);
    }




}
