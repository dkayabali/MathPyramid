using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject rulesPanel;
    public GameObject settingsPanel;
    
    [Header("Buttons")]
    public Button playButton;
    public Button rulesButton;
    public Button settingsButton;
    public Button backFromRulesButton;
    public Button backFromSettingsButton;
    
    [Header("Animation Settings")]
    public float panelAnimationDuration = 0.5f;
    
    void Start()
    {
        // Buton dinleyicilerini ayarla
        playButton.onClick.AddListener(PlayGame);
        rulesButton.onClick.AddListener(ShowRules);
        settingsButton.onClick.AddListener(ShowSettings);
        
        if (backFromRulesButton != null)
            backFromRulesButton.onClick.AddListener(BackToMainMenu);
        
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(BackToMainMenu);
        
        // Başlangıçta sadece ana menü göster
        ShowMainMenu();
    }
    
    public void PlayGame()
    {
        // Oyun sahnesini yükle
        Debug.Log("Oyun başlatılıyor...");
        // SceneManager.LoadScene("GameScene");
    }
    
    public void ShowRules()
    {
        AnimatePanel(mainMenuPanel, false);
        AnimatePanel(rulesPanel, true);
    }
    
    public void ShowSettings()
    {
        AnimatePanel(mainMenuPanel, false);
        AnimatePanel(settingsPanel, true);
    }
    
    public void BackToMainMenu()
    {
        AnimatePanel(rulesPanel, false);
        AnimatePanel(settingsPanel, false);
        AnimatePanel(mainMenuPanel, true);
    }
    
    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        if (rulesPanel != null) rulesPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    
    void AnimatePanel(GameObject panel, bool show)
    {
        if (panel == null) return;
        
        if (show)
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, panelAnimationDuration)
                .SetEase(Ease.OutBack);
        }
        else
        {
            panel.transform.DOScale(Vector3.zero, panelAnimationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => panel.SetActive(false));
        }
    }
}