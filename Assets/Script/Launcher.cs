using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
public class Launcher : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields
    [SerializeField]
    private byte maxPlayersPerRoom = 2;
    #endregion

    #region Private Fields
    string gameVersion = "1";
    bool isConnecting;
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("OnConnectedToMaster 함수 작동");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnceted 발동 원인: ",cause);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed()");
        PhotonNetwork.CreateRoom("Room",new RoomOptions{ MaxPlayers = maxPlayersPerRoom});
    }

    public  override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom()이 작동, 클라이언트 room 접속 완료");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Room에 접속하였습니다.");
        }
        PhotonNetwork.LoadLevel("alpha 1");
    }
    #endregion

    #region MonoBehaviour CallBacks
    // Start is called before the first frame update

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // Debug.Log(isConnecting);
    }
    void Update()
    {
        
    }
    #endregion

    #region Public Methods

    public void Connect()
    {
        isConnecting = true;

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame

    #endregion
}
}