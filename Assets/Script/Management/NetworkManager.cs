using Photon.Pun;               // Photon Unity Networking 기능을 사용하기 위해 필요
using Photon.Realtime;           // Photon Realtime 기능을 사용하기 위해 필요
using System.Collections;        // 컬렉션 라이브러리 사용 (리스트 및 딕셔너리)
using System.Collections.Generic; // 컬렉션 사용을 위한 제네릭 리스트, 딕셔너리 라이브러리 사용
using TMPro;
using UnityEngine;               // Unity 엔진 기능 사용
using UnityEngine.UI;            // Unity UI 기능 사용
using static UnityEngine.EventSystems.PointerEventData;  // 유니티의 이벤트 시스템 사용 (입력 이벤트 관련)

public class NetworkManager : MonoBehaviourPunCallbacks // Photon의 MonoBehaviourPunCallbacks 상속 (Photon 관련 콜백을 받기 위해)
{
   [SerializeField] TMP_InputField RoomName;
   [SerializeField] TMP_InputField MaxRoom;
   [SerializeField] Button ConnectRoom;
   [SerializeField] Button CreateRoom;
   [SerializeField] GameObject roomListItem;
   public Transform rtContent;
   Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

   void Start()
   {
      RoomName.onValueChanged.AddListener(OnNameValueChanged);
      MaxRoom.onValueChanged.AddListener(OnPlayerNumberChange);
      ConnectRoom.onClick.AddListener(OnClickConnectRoom);
      CreateRoom.onClick.AddListener(OnClickCreateRoom);
   }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
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
      foreach(Transform tr in rtContent)
      {
         Destroy(tr.gameObject);
      }
    }
    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
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
      foreach(RoomInfo info in dicRoomInfo.Values)
      {
         GameObject go = Instantiate(roomListItem, rtContent);
         RoomListItem item = go.GetComponent<RoomListItem>();
         item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
         item.onDelegate = SelectRoomItem;
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

   void OnClickCreateRoom(){
      RoomOptions options = new RoomOptions();
      options.MaxPlayers = int.Parse(MaxRoom.text);
      options.IsVisible = true;
      options.IsOpen = true;
      PhotonNetwork.CreateRoom(RoomName.text,options);
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
