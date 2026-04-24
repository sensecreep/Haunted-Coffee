using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки")]
    public TMP_Text buttonText;
    public Color normalColor = new Color(0.761f, 0.761f, 0.761f, 1f); // HEX #C2C2C2
    public Color hoverColor = new Color(0.686f, 0.29f, 0.765f, 1f);   // HEX #AF4AC3
    public float scaleMultiplier = 1.0f;

    private Vector3 originalScale;
    private string originalText; // Храним исходный текст

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        // Сохраняем исходный текст кнопки
        originalText = buttonText.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Изменение стиля + добавление скобок
        buttonText.color = hoverColor;
        buttonText.text = $"[ {originalText} ]"; // Текст в скобках
        transform.localScale = originalScale * scaleMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Возврат к исходному виду
        buttonText.color = normalColor;
        buttonText.text = originalText; // Оригинальный текст без скобок
        transform.localScale = originalScale;
    }
}
