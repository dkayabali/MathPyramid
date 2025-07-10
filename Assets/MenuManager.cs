using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Scene Management")]
    public string gameSceneName = "GameScene";

    void Start()
    {
        // Button event listeners
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        // Menu entrance animation
        AnimateMenuEntrance();
    }

    public void PlayGame()
    {
        Debug.Log("🎮 Play butonuna basıldı! Stage 1'e geçiliyor...");

        // Play button animation
        if (playButton != null)
        {
            playButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f)
                .OnComplete(() => {
                    // Game scene'e geç
                    LoadGameScene();
                });
        }
        else
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        // GameScene'e geç
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("⚙️ Settings açılıyor...");

        // Settings button animation
        if (settingsButton != null)
            settingsButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f);

        // Burada settings paneli açılabilir
        // Şimdilik sadece log
    }

    public void ExitGame()
    {
        Debug.Log("👋 Oyundan çıkılıyor...");

        // Exit button animation
        if (exitButton != null)
        {
            exitButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f)
                .OnComplete(() => {
                    // Uygulamayı kapat
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
        }
    }

    void AnimateMenuEntrance()
    {
        // Butonları başta görünmez yap
        if (playButton != null)
        {
            playButton.transform.localScale = Vector3.zero;
            playButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.2f).SetEase(Ease.OutBack);
        }

        if (settingsButton != null)
        {
            settingsButton.transform.localScale = Vector3.zero;
            settingsButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.4f).SetEase(Ease.OutBack);
        }

        if (exitButton != null)
        {
            exitButton.transform.localScale = Vector3.zero;
            exitButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.6f).SetEase(Ease.OutBack);
        }
    }
}