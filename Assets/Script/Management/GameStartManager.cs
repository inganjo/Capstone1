using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviourPunCallbacks
{

    public TimerManager Timemanager;
    public GameObject startbutton;
    private PhotonView photonView;
    public Button myButton;
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
            startbutton.SetActive(false);
            Timemanager.enabled = true;
            EG.gameObject.SetActive(true);
            Timer.SetActive(true);
        }
        else{
            if(PhotonNetwork.IsMasterClient == false)
            {
                startbutton.SetActive(false);
            
            }
             myButton.onClick.AddListener(() => {
                photonView.RPC("GameStart", RpcTarget.All);
            });
            // if(Input.GetKey(KeyCode.B)){
            //     photonView.RPC("GameStart", RpcTarget.All);
            // }
        }
        
        
    }


    #region RPC
    [PunRPC]
    public void GameStart()
    {
        Debug.Log("게임 시작!");

        startbutton.SetActive(false);
        Timemanager.enabled = true;
        EG.gameObject.SetActive(true);
        Timer.SetActive(true);
    }
    #endregion
}
