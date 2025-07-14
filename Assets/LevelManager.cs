using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public int targetNumber;
    public List<string> letters;
    public List<string> operators;
    public List<int> numbers;
}

[System.Serializable]
public class LevelsContainer
{
    public List<LevelData> levels;
}

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform hexagonParent;
    public GameObject hexagonPrefab;
    public TextMeshProUGUI targetNumberText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI formulaText;
    public Button resetButton;
    public Button nextLevelButton;

    [Header("JSON Data")]
    public TextAsset levelsJsonFile;

    [Header("Pyramid Layout")]
    public Vector2 pyramidCenter = Vector2.zero;

    [Header("Level Settings")]
    public int currentLevelIndex = 0;
    public int maxMoves = 3;

    // Game State
    private LevelsContainer levelsData;
    private LevelData currentLevelData;
    private List<HexagonController> hexagonControllers = new List<HexagonController>();
    private List<HexagonController> selectedHexagons = new List<HexagonController>();
    private bool gameCompleted = false;

    // Pyramid positions (1-2-3-4 layout, A üstte J sol altta)
    private Vector2[] pyramidPositions = new Vector2[]
    {
        // Top row (1 hexagon) - A
        new Vector2(0, 150),
        // Second row (2 hexagons) - B, C
        new Vector2(-100, 0), new Vector2(100, 0),
        // Third row (3 hexagons) - D, E, F
        new Vector2(-200, -150), new Vector2(0, -150), new Vector2(200, -150),
        // Bottom row (4 hexagons) - G, H, I, J
        new Vector2(-300, -300), new Vector2(-100, -300), new Vector2(100, -300), new Vector2(300, -300)
    };

    void Start()
    {
        // Load JSON data
        LoadLevelsFromJSON();

        // Button events
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetLevel);

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
            nextLevelButton.interactable = false;
        }

        // Load first level
        LoadLevel(currentLevelIndex);
    }

    void LoadLevelsFromJSON()
    {
        if (levelsJsonFile == null)
        {
            Debug.LogError("JSON dosyası atanmamış!");
            return;
        }

        try
        {
            string jsonContent = levelsJsonFile.text;
            levelsData = JsonUtility.FromJson<LevelsContainer>(jsonContent);

            Debug.Log($"JSON yüklendi! {levelsData.levels.Count} level bulundu.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON yüklenirken hata: {e.Message}");
        }
    }

    void LoadLevel(int levelIndex)
    {
        if (levelsData == null || levelIndex >= levelsData.levels.Count)
        {
            Debug.LogError("Level data bulunamadı!");
            return;
        }

        // Clear previous hexagons
        ClearHexagons();

        // Reset game state
        selectedHexagons.Clear();
        gameCompleted = false;

        // Get level data from JSON
        currentLevelData = levelsData.levels[levelIndex];

        // Update UI
        UpdateUI();

        // Create hexagons
        CreateHexagons();

        // Reset formula
        UpdateFormulaDisplay();

        // Next Level butonunu her zaman aktif bırak (test için)
        if (nextLevelButton != null)
            nextLevelButton.interactable = true;
    }

    void CreateHexagons()
    {
        for (int i = 0; i < 10; i++) // Her seviyede 10 hexagon
        {
            // Create hexagon
            GameObject hexagonObj = Instantiate(hexagonPrefab, hexagonParent);
            HexagonController hexagonController = hexagonObj.GetComponent<HexagonController>();

            // Position in pyramid
            RectTransform rectTransform = hexagonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = pyramidCenter + pyramidPositions[i];

            // Create hexagon data from JSON
            HexagonData hexagonData = new HexagonData
            {
                letter = currentLevelData.letters[i],
                operation = currentLevelData.operators[i] + currentLevelData.numbers[i].ToString(),
                position = i
            };

            // Setup hexagon
            hexagonController.SetupHexagon(hexagonData);
            hexagonController.OnHexagonClicked += OnHexagonClicked;

            hexagonControllers.Add(hexagonController);
        }
    }

    void OnHexagonClicked(HexagonController hexagon)
    {
        if (gameCompleted) return;

        // Eğer hexagon zaten seçiliyse, seçimi kaldır
        if (selectedHexagons.Contains(hexagon))
        {
            selectedHexagons.Remove(hexagon);
            hexagon.SetSelected(false);
            Debug.Log($"❌ Seçim kaldırıldı: {hexagon.GetLetter()}");
        }
        else
        {
            // Maksimum 3 hexagon seçilebilir
            if (selectedHexagons.Count >= maxMoves)
            {
                Debug.Log("⚠️ Maksimum 3 hexagon seçebilirsiniz!");
                return;
            }

            // Hexagon'u seç
            selectedHexagons.Add(hexagon);
            hexagon.SetSelected(true);
            Debug.Log($"✅ Seçildi: {hexagon.GetLetter()}");
        }

        // Formülü güncelle
        UpdateFormulaDisplay();

        // Eğer 3 hexagon seçildiyse hesapla
        if (selectedHexagons.Count == maxMoves)
        {
            CalculateFormula();
        }
    }

    void UpdateFormulaDisplay()
    {
        if (formulaText == null) return;

        if (selectedHexagons.Count == 0)
        {
            formulaText.text = "Hexagon seç...";
            return;
        }

        string formula = "";

        for (int i = 0; i < selectedHexagons.Count; i++)
        {
            HexagonController hexagon = selectedHexagons[i];

            if (i == 0)
            {
                // İlk hexagon: Sadece sayı
                formula += hexagon.GetOperationValue().ToString();
            }
            else
            {
                // Sonraki hexagonlar: İşaret + sayı
                formula += " " + hexagon.GetOperationSymbol() + " " + hexagon.GetOperationValue().ToString();
            }
        }

        // Eğer 3 hexagon seçildiyse sonucu da göster
        if (selectedHexagons.Count == maxMoves)
        {
            float detailedResult = CalculateDetailedFormulaResult();
            string resultText = detailedResult % 1 == 0 ? detailedResult.ToString("F0") : detailedResult.ToString("F3");
            formula += " = " + resultText;
        }

        formulaText.text = formula;

        // Formula güncelleme animasyonu
        formulaText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5, 0.5f);
    }

    void CalculateFormula()
    {
        if (selectedHexagons.Count != maxMoves) return;

        int result = CalculateFormulaResult();

        Debug.Log($"🧮 Formül hesaplandı: {formulaText.text}");

        // Sonucu kontrol et
        CheckWinCondition(result);

        // Tüm hexagonları kullanılmış olarak işaretle
        foreach (var hexagon in selectedHexagons)
        {
            hexagon.SetUsed(true);
        }

        // Seçili hexagonları temizle
        selectedHexagons.Clear();

        // Formüla animasyonu
        formulaText.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 8, 0.7f);
    }

    int CalculateFormulaResult()
    {
        if (selectedHexagons.Count == 0) return 0;

        // İlk sayı
        float result = selectedHexagons[0].GetOperationValue();

        // İşlem önceliği kuralları: Önce çarpma ve bölme, sonra toplama ve çıkarma
        List<float> numbers = new List<float> { result };
        List<char> operators = new List<char>();

        // Tüm sayıları ve operatörleri topla
        for (int i = 1; i < selectedHexagons.Count; i++)
        {
            operators.Add(selectedHexagons[i].GetOperationSymbol());
            numbers.Add(selectedHexagons[i].GetOperationValue());
        }

        // Önce çarpma ve bölme işlemlerini yap
        for (int i = 0; i < operators.Count; i++)
        {
            char op = operators[i];
            
            // Görsel sembolleri hesaplama sembollerine çevir
            if (op == 'x') op = '*';
            if (op == '÷') op = '/';
            
            if (op == '*' || op == '/')
            {
                float operationResult = 0;

                if (op == '*')
                {
                    operationResult = numbers[i] * numbers[i + 1];
                }
                else if (op == '/' && numbers[i + 1] != 0)
                {
                    operationResult = numbers[i] / numbers[i + 1];
                }
                else
                {
                    operationResult = numbers[i]; // Sıfıra bölme durumunda
                }

                // Sonucu listeye uygula
                numbers[i] = operationResult;
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; // Index'i geri al
            }
        }

        // Sonra toplama ve çıkarma işlemlerini yap
        result = numbers[0];
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '+')
            {
                result += numbers[i + 1];
            }
            else if (operators[i] == '-')
            {
                result -= numbers[i + 1];
            }
        }

        // Sonucu integer'a çevir (hedef sayılar integer olduğu için)
        return Mathf.RoundToInt(result);
    }

    float CalculateDetailedFormulaResult()
    {
        if (selectedHexagons.Count == 0) return 0;

        // İlk sayı
        float result = selectedHexagons[0].GetOperationValue();

        // İşlem önceliği kuralları: Önce çarpma ve bölme, sonra toplama ve çıkarma
        List<float> numbers = new List<float> { result };
        List<char> operators = new List<char>();

        // Tüm sayıları ve operatörleri topla
        for (int i = 1; i < selectedHexagons.Count; i++)
        {
            operators.Add(selectedHexagons[i].GetOperationSymbol());
            numbers.Add(selectedHexagons[i].GetOperationValue());
        }

        // Önce çarpma ve bölme işlemlerini yap
        for (int i = 0; i < operators.Count; i++)
        {
            char op = operators[i];
            
            // Görsel sembolleri hesaplama sembollerine çevir
            if (op == 'x') op = '*';
            if (op == '÷') op = '/';
            
            if (op == '*' || op == '/')
            {
                float operationResult = 0;

                if (op == '*')
                {
                    operationResult = numbers[i] * numbers[i + 1];
                }
                else if (op == '/' && numbers[i + 1] != 0)
                {
                    operationResult = numbers[i] / numbers[i + 1];
                }
                else
                {
                    operationResult = numbers[i]; // Sıfıra bölme durumunda
                }

                // Sonucu listeye uygula
                numbers[i] = operationResult;
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; // Index'i geri al
            }
        }

        // Sonra toplama ve çıkarma işlemlerini yap
        result = numbers[0];
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '+')
            {
                result += numbers[i + 1];
            }
            else if (operators[i] == '-')
            {
                result -= numbers[i + 1];
            }
        }

        return result;
    }

    void CheckWinCondition(int result)
    {
        if (result == currentLevelData.targetNumber)
        {
            gameCompleted = true;

            Debug.Log("🎉 Level Complete!");

            // Show congratulations
            ShowCongratulations();

            // Next Level butonu her zaman aktif (test için)
            // Kazanma koşulu kaldırıldı

            // Win animation
            if (targetNumberText != null)
                targetNumberText.transform.DOPunchScale(Vector3.one * 0.5f, 0.8f, 10, 1f);
        }
        else
        {
            Debug.Log($"❌ Hedef: {currentLevelData.targetNumber}, Sonuç: {result}");
        }
    }

    void ShowCongratulations()
    {
        Debug.Log("🎉 TEBRİKLER! 🎉");
        Debug.Log($"{currentLevelData.levelName} Tamamlandı!");

        // Son level kontrolü
        if (currentLevelIndex >= levelsData.levels.Count - 1)
        {
            Debug.Log("🏆 Tüm levelleri tamamladınız! 🏆");
        }
    }

    void UpdateUI()
    {
        if (targetNumberText != null)
            targetNumberText.text = currentLevelData.targetNumber.ToString();

        if (levelText != null)
            levelText.text = currentLevelData.levelName;
    }

    void ClearHexagons()
    {
        foreach (HexagonController hexagon in hexagonControllers)
        {
            if (hexagon != null)
                Destroy(hexagon.gameObject);
        }

        hexagonControllers.Clear();
        selectedHexagons.Clear();
    }

    public void ResetLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        if (currentLevelIndex < levelsData.levels.Count - 1)
        {
            currentLevelIndex++;
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("Son level! Oyun bitti.");
        }
    }
}