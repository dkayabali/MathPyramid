using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    public Button playButton;
    public Button rulesButton;
    public Button settingsButton;
    
    [Header("Scene Management")]
    public string gameSceneName = "GameScene";
    
void Start()
{
    // Button event listeners
    if (playButton != null)
        playButton.onClick.AddListener(PlayGame);
        
    if (rulesButton != null)
        rulesButton.onClick.AddListener(OpenRules);
        
    if (settingsButton != null)
        settingsButton.onClick.AddListener(OpenSettings);
        
    // Menu entrance animation
    // AnimateMenuEntrance(); // BU SATIRI YORUM YAP
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
    
    public void OpenRules()
    {
        Debug.Log("📖 Rules açılıyor...");
        
        // Rules button animation
        if (rulesButton != null)
            rulesButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f);
            
        // Burada rules paneli açılabilir
        // Şimdilik sadece log
        
        // Gelecekte buraya rules UI eklenebilir:
        // ShowRulesPanel();
    }
    
    public void OpenSettings()
    {
        Debug.Log("⚙️ Settings açılıyor...");
        
        // Settings button animation
        if (settingsButton != null)
            settingsButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f);
            
        // Burada settings paneli açılabilir
        // Şimdilik sadece log
        
        // Gelecekte buraya settings UI eklenebilir:
        // ShowSettingsPanel();
    }
    
    void AnimateMenuEntrance()
    {
        // Butonları başta görünmez yap
        if (playButton != null)
        {
            playButton.transform.localScale = Vector3.zero;
            playButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.2f).SetEase(Ease.OutBack);
        }
        
        if (rulesButton != null)
        {
            rulesButton.transform.localScale = Vector3.zero;
            rulesButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.4f).SetEase(Ease.OutBack);
        }
        
        if (settingsButton != null)
        {
            settingsButton.transform.localScale = Vector3.zero;
            settingsButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.6f).SetEase(Ease.OutBack);
        }
    }
    
    // Gelecekte rules panel için
    void ShowRulesPanel()
    {
        Debug.Log("Rules paneli gösteriliyor...");
        // Rules UI implementation
    }
    
    // Gelecekte settings panel için
    void ShowSettingsPanel()
    {
        Debug.Log("Settings paneli gösteriliyor...");
        // Settings UI implementation
    }
}