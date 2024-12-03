using System.Collections;
using System.Collections.Generic;
using System.Net;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviourPunCallbacks
{


    public GameObject startText;
    private PhotonView photonView;
    public GameObject Timer;
    
    private GameObject Canvas;

    

    private ElevatorGameover EG;
    // Start is called before the first frame update
    void Start()
    { 

        Canvas = TimerManager.instance.transform.GetChild(0).gameObject;
        Timer = Canvas.transform.GetChild(0).gameObject;
        startText = Canvas.transform.GetChild(3).gameObject;
        
        photonView = this.gameObject.GetComponent<PhotonView>();
        EG = FindObjectOfType<ElevatorGameover>();
        EG.gameObject.SetActive(false);
        Timer.SetActive(false);
        if(PhotonNetwork.IsConnected == false){
            startText.SetActive(false);
            TimerManager.instance.enabled = true;
            EG.gameObject.SetActive(true);
            Timer.SetActive(true);
        }
        else{
            if(PhotonNetwork.IsMasterClient == false)
            {
                startText.SetActive(false);
            
            }

        }
        
        
    }


    #region RPC
    [PunRPC]
    public void GameStart()
    {
        Debug.Log("게임 시작!");
        if(PhotonNetwork.IsMasterClient == true)
        {
            Debug.Log("Text 비활성화");
            startText.SetActive(false);
        }
        TimerManager.instance.enabled = true;
        EG.gameObject.SetActive(true);
        Timer.SetActive(true);
    }
    #endregion

    void Update()
    {
        if(Input.GetKey(KeyCode.F1) && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GameStart", RpcTarget.All);
        }
    }
}
