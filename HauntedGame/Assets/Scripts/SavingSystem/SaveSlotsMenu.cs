using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameScene";
    public string introSceneName = "IntroScene";

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

        SaveData data = SaveSystem.Load(slotNumber);

        if (data == null)
        {
            data = new SaveData
            {
                currentDay = 1,
                totalMoney = 0,
                hasSeenIntro = false
            };

            SaveSystem.Save(data, slotNumber);
        }

        //data.totalMoney += 50000;
        //SaveSystem.Save(data, slotNumber);

        SceneManager.LoadScene(data.hasSeenIntro ? gameSceneName : introSceneName);
    }

    public void DeleteSlot(int slotNumber)
    {
        SaveSystem.DeleteSave(slotNumber);
        RefreshSlots();

        Debug.Log("Слот " + slotNumber + " очищен");
    }
}