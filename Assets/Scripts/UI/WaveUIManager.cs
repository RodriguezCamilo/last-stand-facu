using UnityEngine;
using TMPro;
using System.Collections;

public class WaveUIManager : MonoBehaviour
{
    public static WaveUIManager Instance { get; private set; }

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void ShowWave(int waveNumber)
    {
        waveText.text = $"WAVE {waveNumber}";
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    // Efecto Fade
    private IEnumerator FadeRoutine()
    {
        yield return Fade(0, 1, fadeDuration);
        yield return new WaitForSeconds(displayTime);
        yield return Fade(1, 0, fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        canvasGroup.alpha = from;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
