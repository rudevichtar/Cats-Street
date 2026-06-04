using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static void Save(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log($"Игра сохранена в слот {slot}");
    }

    public static SaveData LoadPreview(int slot)
    {
        return Load(slot);
    }

    public static SaveData Load(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Сохранение в слоте {slot} не найдено");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public static void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
            File.Delete(path);
    }
}