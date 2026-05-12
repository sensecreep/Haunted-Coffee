using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot")]
    public int slotNumber;

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI dateText;
    public Button button;

    private SaveSlotsMenu menu;
    public Button deleteButton;

    public void Setup(SaveSlotsMenu menuController)
    {
        menu = menuController;

        SaveData data = SaveSystem.Load(slotNumber);

        if (data == null)
        {
            titleText.text = "New game";

            if (dateText != null)
                dateText.text = "";
            if (deleteButton != null)
                deleteButton.gameObject.SetActive(false);
        }
        else
        {
            titleText.text =
                $"День {data.currentDay} — заработано {data.totalMoney} руб.";

            if (dateText != null)
                dateText.text = data.saveDateTime;

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(true);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSlotClicked);

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }
    }

    void OnDeleteClicked()
    {
        menu.DeleteSlot(slotNumber);
    }

    void OnSlotClicked()
    {
        menu.SelectSlot(slotNumber);
    }
}