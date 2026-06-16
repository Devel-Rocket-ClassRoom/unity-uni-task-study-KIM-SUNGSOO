using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class FadeController : MonoBehaviour
{
    //fade 그룹
    public CanvasGroup fadeCanvasGroup;

    //fade시간
    public float fadeDuration = 1f;

    //중복방지
    private static FadeController instance;


    private void Awake()
    {
        if(instance != null & instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        fadeCanvasGroup.alpha = 0f;

    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    
    public async UniTask FadeOut(float duration = -1f)
    {
        if(duration < 0f)
            duration = fadeDuration;

        await FadeAsync(fadeCanvasGroup, 0f, 1f, duration);
    }
    public async UniTask FadeIn(float duration = -1f)
    {
        if (duration < 0f)
            duration = fadeDuration;

        await FadeAsync(fadeCanvasGroup, 1f, 0f, duration);
    }

    private async UniTask FadeAsync(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        canvasGroup.alpha = from;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t= elapsed/duration;

            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            await UniTask.Yield();
        }
        canvasGroup.alpha = to;
    }
}
