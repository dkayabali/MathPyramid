using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CasualButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    public Color normalColor = new Color(0.3f, 0.7f, 1f, 1f); // Açık mavi
    public Color hoverColor = new Color(0.2f, 0.6f, 0.9f, 1f); // Koyu mavi
    public Color pressedColor = new Color(0.1f, 0.5f, 0.8f, 1f); // Daha koyu mavi
    
    [Header("Animation Settings")]
    public float animationDuration = 0.2f;
    public float hoverScale = 1.05f;
    public float pressScale = 0.95f;
    
    [Header("Shadow Settings")]
    public bool useShadow = true;
    public Vector2 shadowOffset = new Vector2(0, -5);
    public Color shadowColor = new Color(0, 0, 0, 0.3f);
    
    private Image buttonImage;
    private Transform buttonTransform;
    private Vector3 originalScale;
    private bool isPressed = false;
    
    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonTransform = transform;
        originalScale = buttonTransform.localScale;
        
        // Başlangıç rengini ayarla
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
        // Shadow oluştur
        if (useShadow)
        {
            CreateShadow();
        }
        
        // Başlangıç animasyonu
        buttonTransform.localScale = Vector3.zero;
        buttonTransform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
    }
    
    void CreateShadow()
    {
        GameObject shadow = new GameObject("Shadow");
        shadow.transform.SetParent(transform.parent);
        shadow.transform.SetSiblingIndex(transform.GetSiblingIndex());
        
        Image shadowImage = shadow.AddComponent<Image>();
        shadowImage.sprite = buttonImage.sprite;
        shadowImage.color = shadowColor;
        
        RectTransform shadowRect = shadow.GetComponent<RectTransform>();
        RectTransform buttonRect = GetComponent<RectTransform>();
        
        shadowRect.anchorMin = buttonRect.anchorMin;
        shadowRect.anchorMax = buttonRect.anchorMax;
        shadowRect.sizeDelta = buttonRect.sizeDelta;
        shadowRect.anchoredPosition = buttonRect.anchoredPosition + shadowOffset;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPressed)
        {
            // Renk animasyonu
            if (buttonImage != null)
            {
                buttonImage.DOColor(hoverColor, animationDuration);
            }
            
            // Scale animasyonu
            buttonTransform.DOScale(originalScale * hoverScale, animationDuration)
                .SetEase(Ease.OutQuart);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPressed)
        {
            // Normal renge dön
            if (buttonImage != null)
            {
                buttonImage.DOColor(normalColor, animationDuration);
            }
            
            // Normal scale'e dön
            buttonTransform.DOScale(originalScale, animationDuration)
                .SetEase(Ease.OutQuart);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        
        // Basılma efekti
        if (buttonImage != null)
        {
            buttonImage.DOColor(pressedColor, animationDuration * 0.5f);
        }
        
        buttonTransform.DOScale(originalScale * pressScale, animationDuration * 0.5f)
            .SetEase(Ease.OutQuart);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        
        // Normal duruma dön
        if (buttonImage != null)
        {
            buttonImage.DOColor(normalColor, animationDuration);
        }
        
        buttonTransform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutBack);
    }
    
    // Buton tıklama sesi için
    public void PlayClickSound()
    {
        // Buraya ses efekti kodu eklenebilir
        Debug.Log("Button clicked!");
    }
}