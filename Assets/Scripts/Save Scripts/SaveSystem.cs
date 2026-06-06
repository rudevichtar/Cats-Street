using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private static string SaveFolder => Application.streamingAssetsPath;

    private static string GetSavePath(int slot)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        return Path.Combine(SaveFolder, $"save_slot_{slot}.json");
    }

    public static void Save(SaveData data, int slot)
    {
        string path = GetSavePath(slot);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json, Encoding.UTF8);

        Debug.Log($"Игра сохранена в слот {slot}: {path}");
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
            Debug.LogWarning($"Сохранение в слоте {slot} не найдено: {path}");
            return null;
        }

        string json = File.ReadAllText(path, Encoding.UTF8);
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
        {
            File.Delete(path);
            Debug.Log($"Сохранение удалено из слота {slot}: {path}");
        }
    }

    public static string GetSaveFolderPath()
    {
        return SaveFolder;
    }
}