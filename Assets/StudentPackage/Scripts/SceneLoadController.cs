using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadController : MonoBehaviour
{

    public Button loadSceneButton; //씬전환 버튼
    public Slider progressSlider; //슬라이더 
    public TMP_Text progressText; // 퍼센트 텍스트

    public FadeController fadeController;

    public string targetSceneName;

    public float minimumLoadTime = 2f; //씬 로드 최소 시간 확보

    private bool isLoading = false;

    private void Start()
    {
        loadSceneButton.onClick.AddListener(OnLoadSceneClicked);

        fadeController = FindFirstObjectByType<FadeController>();
        gameObject.SetActive(false);
    }

    private void OnLoadSceneClicked()
    {
        LoadSceneWithAsync().Forget();
    }
    private async UniTask LoadSceneWithAsync()
    {
        progressSlider.value = 0f;
        progressText.text = $"로딩 중 ...0%";
        
        await fadeController.FadeOut();

        gameObject.SetActive(true);
        isLoading = true;
        loadSceneButton.interactable = false;
        await LoadSceneAsync();

        await fadeController.FadeIn();

        isLoading = false;
        //loadSceneButton.interactable = true;
    }
    private async UniTask LoadSceneAsync()
    {
        var progress = Progress.Create<float>(p =>
        {
            progressSlider.value = p;
            progressText.text = $"로딩중 ...{(int)(p * 100)}%";
            Debug.Log($"로딩중 ...{(int)(p * 100)}%");
        }
        );

        var asyncOp = SceneManager.LoadSceneAsync(targetSceneName);
        asyncOp.allowSceneActivation = false;

        float startTime = Time.time;

        while(asyncOp.progress < 0.9f)
        {
            progress.Report(Mathf.Clamp01(asyncOp.progress / 0.9f));
            await UniTask.Yield();
        }

        float elapsed = Time.time - startTime;
        if(elapsed < minimumLoadTime)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(minimumLoadTime-elapsed));

        }
        progress.Report(1.0f);
        asyncOp.allowSceneActivation = true;
        await asyncOp.ToUniTask();

    }
}
