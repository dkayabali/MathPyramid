using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class HexagonData
{
    public string letter;
    public string operation;
    public int position;
}

public class HexagonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image hexagonImage;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI operationText;
    
    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.3f, 0.8f, 1f);
    public Color hoverColor = new Color(0.3f, 0.4f, 0.9f, 1f);
    public Color selectedColor = new Color(0.8f, 0.3f, 0.2f, 1f);
    public Color usedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    
    [Header("Animation")]
    public float animationDuration = 0.2f;
    public float hoverScale = 1.1f;
    
    // Data
    private HexagonData hexagonData;
    private bool isSelected = false;
    private bool isUsed = false;
    private Vector3 originalScale;
    
    // Events
    public System.Action<HexagonController> OnHexagonClicked;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        // Başlangıç animasyonu
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 0.5f)
            .SetEase(Ease.OutBack)
            .SetDelay(hexagonData != null ? hexagonData.position * 0.1f : 0f);
    }
    
    public void SetupHexagon(HexagonData data)
    {
        hexagonData = data;
        
        if (letterText != null)
            letterText.text = data.letter;
            
        if (operationText != null)
        {
            string operation = data.operation;
            
            // x işaretini küçült
            if (operation.Contains("x"))
            {
                operation = operation.Replace("x", "<size=80%>x</size>");
            }
            
            operationText.text = operation;
        }
            
        if (hexagonImage != null)
            hexagonImage.color = normalColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed) return;
        
        OnHexagonClicked?.Invoke(this);
        
        // Tıklama animasyonu
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUsed) return;
        
        // Hover efekti
        if (hexagonImage != null)
        {
            hexagonImage.DOColor(hoverColor, animationDuration);
        }
        
        transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(Ease.OutQuart);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUsed) return;
        
        Color targetColor = isSelected ? selectedColor : normalColor;
        
        if (hexagonImage != null)
        {
            hexagonImage.DOColor(targetColor, animationDuration);
        }
        
        transform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutQuart);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        Color targetColor = selected ? selectedColor : normalColor;
        
        if (hexagonImage != null)
        {
            hexagonImage.DOColor(targetColor, animationDuration);
        }
    }
    
    public void SetUsed(bool used)
    {
        isUsed = used;
        
        if (used)
        {
            if (hexagonImage != null)
            {
                hexagonImage.DOColor(usedColor, animationDuration);
            }
            
            // Kullanıldığında küçült
            transform.DOScale(originalScale * 0.8f, animationDuration)
                .SetEase(Ease.OutQuart);
                
            // Saydamlık efekti
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                
            canvasGroup.DOFade(0.5f, animationDuration);
        }
    }
    
    public string GetOperation()
    {
        return hexagonData?.operation ?? "";
    }
    
    public string GetLetter()
    {
        return hexagonData?.letter ?? "";
    }
    
    public int GetOperationValue()
    {
        if (string.IsNullOrEmpty(hexagonData?.operation)) return 0;
        
        string op = hexagonData.operation;
        string numberStr = op.Substring(1); // İlk karakteri (işareti) atla
        
        if (int.TryParse(numberStr, out int value))
        {
            return value;
        }
        
        return 0;
    }
    
    public char GetOperationSymbol()
    {
        if (string.IsNullOrEmpty(hexagonData?.operation)) return '+';
        return hexagonData.operation[0];
    }
    
    void OnDestroy()
    {
        transform.DOKill();
        if (hexagonImage != null) hexagonImage.DOKill();
    }
}