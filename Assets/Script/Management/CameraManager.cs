using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform cameraRoot;

    private CinemachineVirtualCamera playerCamera; // Virtual Camera 참조
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        if (photonView.IsMine) // 현재 클라이언트의 플레이어라면
        {
            // Follow와 LookAt을 현재 플레이어로 설정
            playerCamera.Follow = cameraRoot;
            playerCamera.LookAt = cameraRoot;
        }
        else
        {
            // 다른 플레이어에게는 카메라가 연결되지 않도록 설정
            playerCamera.gameObject.SetActive(false); 
        }
    }
}

