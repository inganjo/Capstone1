using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;  // 전환할 씬의 이름

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 태그가 "Player"인 오브젝트가 트리거에 들어왔을 때
        {
            FindObjectOfType<SceneFader>().FadeToScene(sceneName);
        }
    }
}
