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

    // Data
    private HexagonData hexagonData;
    private bool isSelected = false;
    private bool isUsed = false;

    // Events
    public System.Action<HexagonController> OnHexagonClicked;

    void Start()
    {
        // Başlangıç animasyonu
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f)
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

        // Basit tıklama efekti
        transform.DOPunchPosition(Vector3.up * 5f, 0.2f, 5, 0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUsed) return;

        // Sadece renk değişir
        if (hexagonImage != null)
        {
            hexagonImage.DOColor(hoverColor, animationDuration);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUsed) return;

        // Doğru renge dön
        Color targetColor = isSelected ? selectedColor : normalColor;

        if (hexagonImage != null)
        {
            hexagonImage.DOColor(targetColor, animationDuration);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (isUsed) return; // Used ise renk değiştirme

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
            // Kullanıldığında: Seçili rengi koru (yeşil kalsın)
            // Saydamlık efekti yok - sadece yeşil renkte kalır
        }
        else
        {
            // Reset: Normal duruma dön
            isSelected = false; // Seçili durumu temizle

            if (hexagonImage != null)
            {
                hexagonImage.DOKill();
                hexagonImage.DOColor(normalColor, animationDuration); // Normal renge dön
            }

            // Saydamlığı normale dön (eğer varsa)
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOKill();
                canvasGroup.DOFade(1f, animationDuration);
            }
        }
    }

    public bool IsUsed()
    {
        return isUsed;
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
        string operation = hexagonData.operation;

        if (operation.Contains("x"))
            return 'x';
        else if (operation.Contains("÷"))
            return '÷';
        else if (operation.Contains("+"))
            return '+';
        else if (operation.Contains("-"))
            return '-';

        return '+'; // Default
    }

    void OnDestroy()
    {
        transform.DOKill();
        if (hexagonImage != null) hexagonImage.DOKill();
    }
}