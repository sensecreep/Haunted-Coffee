using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonEffectImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки изображения")]
    public Image targetImage;

    [Header("Обычное состояние")]
    public Sprite normalSprite;
    public Color normalColorImage = Color.black;

    [Header("Состояние при наведении")]
    public Sprite hoverSprite;
    public Color hoverColorImage = Color.white;

    private void Start()
    {
        // Инициализация начального состояния
        if (targetImage != null)
        {
            targetImage.sprite = normalSprite;
            targetImage.color = normalColorImage;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage == null) return;

        // Меняем и спрайт, и цвет
        targetImage.sprite = hoverSprite;
        targetImage.color = hoverColorImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage == null) return;

        // Возвращаем исходные значения
        targetImage.sprite = normalSprite;
        targetImage.color = normalColorImage;
    }
}