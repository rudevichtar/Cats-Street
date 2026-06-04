using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomName
{
    private static string[] catNames =
    {
        "Мурка",
        "Барсик",
        "Bruh",
        "Луна",
        "Том",
        "Симба",
        "Чепух",
        "Милка",
        "Котость",
        "Вязанка",
        "Плюшка",
        "Воришка",
        "Герцог",
        "Граф",
        "Малёк",
        "Лапа",
        "Мила",
        "Шипучка",
        "Кислинка",
        "Лада"
    };

    public static string GetRandomCatName()
    {
        int index = Random.Range(0, catNames.Length);
        return catNames[index];
    }
}
