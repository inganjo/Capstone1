using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerUIManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject playerUI;
    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + "\'s ui active");
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            playerUI.SetActive(true);
        }
        else
        {
            playerUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
