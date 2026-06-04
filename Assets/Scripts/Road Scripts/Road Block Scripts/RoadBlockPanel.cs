using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Работа панели для закрытия/открытия дороги
public class RoadBlockPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelContent;
    [SerializeField] private TMP_Text roadNameText;

    [Header("Toggle")]
    [SerializeField] private Toggle roadToggle;
    [SerializeField] private TMP_Text toggleLabel;

    private RoadSwitch selectedRoad;
    private bool isUpdatingUI = false;

    private void Start()
    {
        panelContent.SetActive(false);

        RoadSelectionManager.Instance.OnRoadSelected += OnRoadSelected;

        roadToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        if (RoadSelectionManager.Instance != null)
            RoadSelectionManager.Instance.OnRoadSelected -= OnRoadSelected;
    }

    private void OnRoadSelected(RoadSwitch road)
    {
        selectedRoad = road;

        if (selectedRoad == null)
        {
            panelContent.SetActive(false);
            return;
        }

        panelContent.SetActive(true);
        Refresh();
    }

    private void Refresh()
    {
        if (selectedRoad == null)
        {
            if (panelContent != null)
                panelContent.SetActive(false);

            return;
        }

        isUpdatingUI = true;

        if (roadNameText != null)
            roadNameText.text = selectedRoad.DisplayName;

        bool isOpen = !selectedRoad.IsBlocked();

        if (roadToggle != null)
            roadToggle.isOn = isOpen;

        if (toggleLabel != null)
        {
            toggleLabel.text = isOpen
                ? "Дорога открыта"
                : "Дорога закрыта";
        }

        isUpdatingUI = false;
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isUpdatingUI || selectedRoad == null)
            return;

        bool shouldBeBlocked = !isOn;

        RoadBlockSystem.Instance.SetBlockedBidirectional(
            selectedRoad.From,
            selectedRoad.To,
            shouldBeBlocked
        );

        Refresh();

        if (shouldBeBlocked)
            Invoke(nameof(Refresh), 5.05f);
    }
}
