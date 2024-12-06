using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class SceneChanger : MonoBehaviourPunCallbacks
{
    public string sceneName;  // 전환할 씬의 이름
    public Vector3 targetPosition;  // 씬 전환 후 이동할 좌표
    private bool isTransitioning = false;  // 중복 전환 방지 플래그

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)  // "Player" 태그와 중복 전환 방지 확인
        {
            isTransitioning = true;  // 플래그 설정
            SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 이벤트 등록

            SceneFader sceneFader = FindObjectOfType<SceneFader>();
            if (sceneFader != null)
            {
                sceneFader.FadeToScene(sceneName);  // 페이드 아웃 후 씬 전환
            }
            else
            {
                Debug.LogWarning("SceneFader component not found. Directly loading the scene.");
                SceneManager.LoadScene(sceneName);  // SceneFader가 없을 경우 직접 씬 로드
            }
        }
    }

    // 씬이 로드된 후 호출될 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneName)  // 현재 로드된 씬이 목표 씬인지 확인
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject player in players)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (player != null )
                {
                    if(photonView.IsMine || PhotonNetwork.IsConnected == false)
                    player.transform.position = targetPosition;  // 플레이어 위치 이동
                }   
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;  // 이벤트 등록 해제
        }
    }
}
