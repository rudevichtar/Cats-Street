using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupHint : MonoBehaviour
{
    public static PopupHint Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showTime = 2f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (panel == null || messageText == null)
            return;

        messageText.text = message;
        panel.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(showTime);

        panel.SetActive(false);
    }
}
