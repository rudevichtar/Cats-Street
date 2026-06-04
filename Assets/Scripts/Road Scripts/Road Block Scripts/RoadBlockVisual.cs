using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockVisual : MonoBehaviour
{
    public void Play(bool blocked)
    {
        Debug.Log(blocked ? "Дорога закрыта" : "Дорога открыта");
    }
}
