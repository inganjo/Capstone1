using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;  // 전환할 씬의 이름
    public Vector3 targetPosition;  // 씬 전환 후 이동할 좌표

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 태그가 "Player"인 오브젝트가 트리거에 들어왔을 때
        {
            // 씬 전환 후 로드될 씬에서 플레이어의 이동 좌표를 기억하도록 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;

            // 씬 전환
            FindObjectOfType<SceneFader>().FadeToScene(sceneName);
        }
    }

    // 씬이 로드된 후 호출될 메소드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 로드된 씬이 목표 씬인지 확인
        if (scene.name == sceneName)
        {
            // 플레이어를 찾아서 위치를 이동
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = targetPosition;
            }

            // 씬 로드 이벤트 해제
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
