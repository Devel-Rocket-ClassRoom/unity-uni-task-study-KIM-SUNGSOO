using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public TMP_Text timer_Text;
    public TMP_Text status_Text;

    public Button strBtn;
    public Button pauseBtn;
    public Button contBtn;
    public Button rstBtn;

    private int seconds = 0;
    private int minutes = 0;
    private bool isRunning = false;
    private CancellationTokenSource timerCts;

    private void Awake()
    {
        status_Text.text = "준비";
        pauseBtn.interactable = false;
        contBtn.interactable = false;
        rstBtn.interactable = false;

        strBtn.onClick.AddListener(Onstart);
        pauseBtn.onClick.AddListener(OnPause);
        contBtn.onClick.AddListener(OnContinue);
        rstBtn.onClick.AddListener(OnReset);
    }

    private void OnDestroy()
    {
        timerCts?.Cancel();
        timerCts?.Dispose();
    }

    private void Onstart()
    {
        if (isRunning) return;
        status_Text.text = "실행 중";
        strBtn.interactable = false;
        pauseBtn.interactable = true;
        contBtn.interactable = false;
        rstBtn.interactable = false;
        timerCts = new CancellationTokenSource();
        isRunning = true;
        TimerAsync(timerCts.Token).Forget();
        
    }
    private void OnPause()
    {
        timerCts.Cancel();
        status_Text.text = "일시정지";
        pauseBtn.interactable = false;
        contBtn.interactable = true;
        rstBtn.interactable = true;
        isRunning = false;
    }
    private void OnContinue()
    {
        if (isRunning) return;
        status_Text.text = "실행 중";
        pauseBtn.interactable = true;
        contBtn.interactable = false;
        rstBtn.interactable = false;
        timerCts = new CancellationTokenSource();
        isRunning = true;
        TimerAsync(timerCts.Token).Forget();
    }
    private void OnReset()
    {
        strBtn.interactable = true;
        pauseBtn.interactable = false;
        contBtn.interactable = false;
        rstBtn.interactable = false;
        seconds = 0;
        minutes = 0;
        isRunning = false;
        timer_Text.text = "00:00";
        status_Text.text = "준비";
    }
    private async UniTaskVoid TimerAsync(CancellationToken ct)
    {
        try
        {
            while(true)
            {
                await UniTask.Delay(1000 , cancellationToken: ct);
                seconds++;
                if (seconds >= 60)
                {
                    seconds = 0;
                    minutes++;

                }
                timer_Text.text = $"{minutes:D2}:{seconds:D2}";
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
}
