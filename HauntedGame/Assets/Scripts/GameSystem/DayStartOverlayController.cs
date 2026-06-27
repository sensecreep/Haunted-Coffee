using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DayStartOverlayController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI dreamText;

    [Header("Dream Goal")]
    [SerializeField] private int moneyGoal = 50000;

    [Header("Timing")]
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        UpdateTexts();

        StartCoroutine(ShowAndFadeRoutine());
    }

    private void UpdateTexts()
    {
        int currentDay = 1;
        int totalMoney = 0;

        if (GameProgressManager.Instance != null)
        {
            currentDay = GameProgressManager.Instance.CurrentDay;
            totalMoney = GameProgressManager.Instance.TotalMoney;
        }
        else
        {
            SaveData data = SaveSystem.Load(SaveSystem.SelectedSlot);

            if (data != null)
            {
                currentDay = data.currentDay;
                totalMoney = data.totalMoney;
            }
        }

        int moneyLeft = Mathf.Max(0, moneyGoal - totalMoney);

        if (dayText != null)
            dayText.text = "День " + currentDay;

        if (dreamText != null)
            dreamText.text = "До мечты осталось " + FormatMoney(moneyLeft) + " рублей";
    }

    private IEnumerator ShowAndFadeRoutine()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return new WaitForSecondsRealtime(showDuration);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
    }
}