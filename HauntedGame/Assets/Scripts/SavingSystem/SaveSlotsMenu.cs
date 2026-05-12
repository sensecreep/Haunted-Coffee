using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Slots")]
    public SaveSlotUI[] slots;

    private void OnEnable()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        foreach (SaveSlotUI slot in slots)
        {
            slot.Setup(this);
        }
    }

    public void SelectSlot(int slotNumber)
    {
        SaveSystem.SelectedSlot = slotNumber;

        // Если сохранения нет — создаём новую игру
        if (!SaveSystem.HasSave(slotNumber))
        {
            SaveData newSave = new SaveData
            {
                currentDay = 1,
                totalMoney = 0
            };

            SaveSystem.Save(newSave, slotNumber);
        }

        SceneManager.LoadScene(gameSceneName);
    }
    public void DeleteSlot(int slotNumber)
    {
        SaveSystem.DeleteSave(slotNumber);
        RefreshSlots();

        Debug.Log("Слот " + slotNumber + " очищен");
    }
}