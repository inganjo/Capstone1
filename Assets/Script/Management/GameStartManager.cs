using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviourPunCallbacks
{

    public TimerManager Timemanager;
    public GameObject startText;
    private PhotonView photonView;
    public TMP_Text StartAlarm;
    public GameObject Timer;

    private ElevatorGameover EG;
    // Start is called before the first frame update
    void Start()
    {
        photonView = this.gameObject.GetComponent<PhotonView>();
        EG = FindObjectOfType<ElevatorGameover>();
        EG.gameObject.SetActive(false);
        Timer.SetActive(false);
        if(PhotonNetwork.IsConnected == false){
            startText.SetActive(false);
            Timemanager.enabled = true;
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
        if(PhotonNetwork.IsMasterClient == false)
        {
            startText.SetActive(false);
        }
        Timemanager.enabled = true;
        EG.gameObject.SetActive(true);
        Timer.SetActive(true);
    }
    #endregion

    void update()
    {
        if(Input.GetKey(KeyCode.F1) && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GameStart", RpcTarget.All);
        }
    }
}
