using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 1;
    }
}
