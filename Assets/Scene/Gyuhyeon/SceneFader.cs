using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;  // 페이드 효과를 적용할 이미지
    public float fadeDuration = 1f;  // 페이드가 걸리는 시간
    private bool isFading = false;  // 페이드 중인지 확인하는 변수

    private void Start()
    {
        // 씬이 시작될 때 화면을 페이드 인 시킴
        StartCoroutine(FadeIn());
    }

    // 씬을 이동할 때 호출하는 함수
    public void FadeToScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndChangeScene(sceneName));
        }
    }

    // 페이드 아웃 후 씬을 전환하는 코루틴
    IEnumerator FadeOutAndChangeScene(string sceneName)
    {
        isFading = true;

        // 페이드 아웃 (화면이 서서히 어두워짐)
        yield return StartCoroutine(Fade(1f));

        // 씬을 전환
        SceneManager.LoadScene(sceneName);

        fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        isFading = false;
    }

    // 페이드 인 (화면 밝아지기) 코루틴
    IEnumerator FadeIn()
    {
        yield return Fade(0f);
    }

    // 페이드 효과를 적용하는 코루틴
    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;  // 현재 알파 값
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            yield return null;
        }

        // 마지막에 정확한 알파 값으로 설정
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
}