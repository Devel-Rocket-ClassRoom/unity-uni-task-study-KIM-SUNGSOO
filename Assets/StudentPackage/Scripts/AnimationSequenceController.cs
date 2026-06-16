using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSequenceController : MonoBehaviour
{
    public Button startBtn;
    public Button resetBtn;
    public Button cancelBtn;

    public RectTransform box;
    public CanvasGroup canvasGroup;
    public TMP_Text statusText;

    private CancellationTokenSource animCts;
    private bool isPlaying = false;

    private Vector2 originPos;
    private Vector3 originScale;
    private Quaternion originRotation;

    private void Awake()
    {
        startBtn.onClick.AddListener(OnStart);
        resetBtn.onClick.AddListener(OnReset);
        cancelBtn.onClick.AddListener(OnCancel);

        resetBtn.interactable = false;
        cancelBtn.interactable = false;

        originPos = box.anchoredPosition;
        originScale = box.localScale;
        originRotation = box.localRotation;

        statusText.text = "대기";
    }

    private void OnDestroy()
    {
        animCts?.Cancel();
        animCts?.Dispose();
    }

    private void OnStart()
    {
        if (isPlaying) return;

        animCts = new CancellationTokenSource();
        isPlaying = true;

        startBtn.interactable = false;
        resetBtn.interactable = false;
        cancelBtn.interactable = true;

        PlayAsync(animCts.Token).Forget();
    }

    private void OnCancel()
    {
        animCts?.Cancel();
        isPlaying = false;

        startBtn.interactable = true;
        resetBtn.interactable = true;
        cancelBtn.interactable = false;

        statusText.text = "정지";
    }

    private void OnReset()
    {
        animCts?.Cancel();
        isPlaying = false;

        box.anchoredPosition = originPos;
        box.localScale = originScale;
        box.localRotation = originRotation;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        startBtn.interactable = true;
        resetBtn.interactable = false;
        cancelBtn.interactable = false;

        statusText.text = "대기";
    }

    private async UniTaskVoid PlayAsync(CancellationToken ct)
    {
        try
        {
            statusText.text = "이동 중...";
            await FadeInAsync(ct);

            await MoveRightAsync(ct);

            await ScaleUpAsync(ct);

            await RotateAsync(ct);

            statusText.text = "완료!";
            startBtn.interactable = false;
            resetBtn.interactable = true;
            cancelBtn.interactable = false;
            isPlaying = false;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async UniTask FadeInAsync(CancellationToken ct)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield(cancellationToken: ct);
        }

        canvasGroup.alpha = 1f;
    }

    private async UniTask MoveRightAsync(CancellationToken ct)
    {
        Vector2 from = box.anchoredPosition;
        Vector2 to = from + Vector2.right * 200f;
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            box.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            await UniTask.Yield(cancellationToken: ct);
        }

        box.anchoredPosition = to;
    }

    private async UniTask ScaleUpAsync(CancellationToken ct)
    {
        Vector3 from = box.localScale;
        Vector3 to = from * 1.5f;
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            box.localScale = Vector3.Lerp(from, to, elapsed / duration);
            await UniTask.Yield(cancellationToken: ct);
        }

        box.localScale = to;
    }

    private async UniTask RotateAsync(CancellationToken ct)
    {
        float startAngle = box.eulerAngles.z;
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0, 360f, elapsed / duration);
            box.rotation = Quaternion.Euler(0, 0, startAngle + angle);
            await UniTask.Yield(cancellationToken: ct);
        }

        box.rotation = Quaternion.Euler(0, 0, startAngle + 360f);
    }
}
