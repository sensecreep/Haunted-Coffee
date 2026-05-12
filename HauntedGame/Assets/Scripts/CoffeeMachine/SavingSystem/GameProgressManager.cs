using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    public int CurrentDay { get; private set; } = 1;
    public int TotalMoney { get; private set; } = 0;

    private void Awake()
    {
        Instance = this;
        LoadProgress();
    }

    void LoadProgress()
    {
        SaveData data = SaveSystem.Load(SaveSystem.SelectedSlot);

        if (data == null)
        {
            data = new SaveData();
            SaveSystem.Save(data, SaveSystem.SelectedSlot);
        }

        CurrentDay = data.currentDay;
        TotalMoney = data.totalMoney;

        Debug.Log($"Загружено: день {CurrentDay}, всего денег {TotalMoney}");
    }

    public void CompleteDay(int earnedToday)
    {
        TotalMoney += earnedToday;
        CurrentDay++;

        SaveProgress();
    }

    public void SaveProgress()
    {
        SaveData data = new SaveData
        {
            currentDay = CurrentDay,
            totalMoney = TotalMoney
        };

        SaveSystem.Save(data, SaveSystem.SelectedSlot);
    }
}