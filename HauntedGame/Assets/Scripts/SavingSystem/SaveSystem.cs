using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SelectedSlotKey = "SelectedSaveSlot";

    public static int SelectedSlot
    {
        get => PlayerPrefs.GetInt(SelectedSlotKey, 1);
        set
        {
            PlayerPrefs.SetInt(SelectedSlotKey, value);
            PlayerPrefs.Save();
        }
    }

    static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static void Save(SaveData data, int slot)
    {
        data.saveDateTime = DateTime.Now.ToString("dd/MM/yy, HH:mm");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);

        Debug.Log("Сохранено в слот " + slot);
    }

    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    public static void DeleteSave(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
            File.Delete(path);
    }
}