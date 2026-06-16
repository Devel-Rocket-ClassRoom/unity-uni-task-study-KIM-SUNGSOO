using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AutoSaveController : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button manualSaveButton;
    public TMP_Text lastSaveText;
    public TMP_Text statusText;
    public Toggle autoSaveToggle;
    public float saveDuration = 2f;

    private CancellationTokenSource debounceCts;

    private bool isSaving = false;

    private string saveData = string.Empty;

    private void Start()
    {
        manualSaveButton.onClick.AddListener(OnClickManualSave);
        inputField.onValueChanged.AddListener(OnInputChanged);
        autoSaveToggle.isOn = true;
        autoSaveToggle.onValueChanged.AddListener(OnAutoSaveToggleChanged);
        AutoSaveLoop().Forget();

        lastSaveText.text = string.Empty;
        statusText.text = "준비";
    }

    private void OnDestroy()
    {
        debounceCts?.Cancel();
        debounceCts?.Dispose();
    }
    private void OnInputChanged(string text)
    {
        debounceCts?.Cancel();
        debounceCts?.Dispose();
        debounceCts = new CancellationTokenSource();

        DebounceAndSave(debounceCts.Token).Forget();
    }

    public float debounceInterval = 3f;
    private async UniTaskVoid DebounceAndSave(CancellationToken ct)
    {
        try
        {
            statusText.text = "추가 입력 대기 중...";
            await UniTask.Delay(TimeSpan.FromSeconds(debounceInterval),cancellationToken : ct);
            await SaveData("디바운스 저장");
        }
        catch(OperationCanceledException)
        {

        }
    }
    private void OnAutoSaveToggleChanged(bool value)
    {
        statusText.text = value ? "자동 저장 켜짐" : "자동 저장 꺼짐";
    }
    private void OnClickManualSave()
    {
        SaveData("수동 저장").Forget();
    }

    public float autoSaveInterval = 10f;
    private async UniTaskVoid AutoSaveLoop()
    {
        var ct = this.GetCancellationTokenOnDestroy();
        
            try
            {
                while(true)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(autoSaveInterval), cancellationToken: ct);
                    if (autoSaveToggle.isOn)
                    {
                        await SaveData("자동 저장");
                    }
                }
                
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Auto save loop Cancelled!");
            }
        
        
    }
    private async UniTask SaveData(string message)
    {
        if(isSaving)
        {
            statusText.text = "저장 중. 건너뜀";
            return;
        }

        isSaving = true;
        try
        {
            statusText.text = "저장 중....";
            await UniTask.Delay(TimeSpan.FromSeconds(saveDuration));
            saveData = inputField.text;
            statusText.text = $"{message}: 저장 완료...";
            lastSaveText.text = $"마지막 저장 시간: {DateTime.Now:HH:mm:ss}";
        }
        catch(Exception ex)
        {
            statusText.text = "저장 실패";
        }
        finally
        {
            isSaving = false;

        }
    }
}
