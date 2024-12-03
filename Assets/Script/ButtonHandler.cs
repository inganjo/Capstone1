using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ButtonHandler : MonoBehaviourPun
{
    // Start is called before the first frame update
    public Button myButton;

    void Start()
    {
        myButton.onClick.AddListener(() => {
            photonView.RPC("PerformAction", RpcTarget.All);
        });
    }
}
