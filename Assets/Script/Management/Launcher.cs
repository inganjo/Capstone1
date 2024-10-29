using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace Com.MyCompany.MyGame
{
public class Launcher : MonoBehaviourPunCallbacks
{

    #region Private Serializable Fields

    [SerializeField]
    private TMP_InputField NickName_input;
    #endregion

    #region Private Fields
    string gameVersion = "1";

    #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("OnConnectedToMaster 작동: 마스터 서버 접속");
            PhotonNetwork.JoinLobby();
        }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnceted 발동 원인: ",cause);
    }
  

    public  override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("OnJoinedLobby(): 로비로 이동합니다.");
        PhotonNetwork.NickName = NickName_input.text;
        PhotonNetwork.LoadLevel("Lobby");
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

            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame

    #endregion
}
}