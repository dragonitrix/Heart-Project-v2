using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HComponentButton : MonoBehaviour
{
    public bool isSelected = false;
    public bool isChecked = false;
    public string componentName;
    public int componentIndex;

    [Header("obj ref")]
    public TextMeshProUGUI componentNameText;
    public Image selectedBG;
    public Image selectedPointer;
    public Image checkMark;
    public EventTrigger eventTrigger;

    public CanvasGroup highlightIcon;


    public void Start()
    {
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((pointerData) => { OnPointerEnter(); });

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((pointerData) => { OnPointerExit(); });

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((pointerData) => { OnPointerClick(); });

        eventTrigger.triggers.Add(enterEntry);
        eventTrigger.triggers.Add(exitEntry);
        eventTrigger.triggers.Add(clickEntry);


    }

    public void SetData(string name, int index)
    {
        componentName = name;
        componentNameText.text = name;
        componentIndex = index;
    }

    public void OnPointerEnter()
    {
        if (!isSelected) selectedBG.color = new Color(255, 255, 255, 0.5f);
    }
    public void OnPointerExit()
    {
        if (!isSelected) selectedBG.color = new Color(255, 255, 255, 0.0f);
    }

    public void Select(bool val)
    {
        isSelected = val;
        if (isSelected)
        {
            selectedPointer.color = new Color(255, 255, 255, 1f);
            selectedBG.color = new Color(255, 255, 255, 1f);
            checkMark.color = new Color(255, 255, 255, 0.0f);
        }
        else
        {
            selectedPointer.color = new Color(255, 255, 255, 0f);
            selectedBG.color = new Color(255, 255, 255, 0.0f);
            checkMark.color = new Color(255, 255, 255, isChecked ? 1f : 0.0f);
        }
    }

    public void OnPointerClick()
    {
        GameManager.instance.SelectHeartComponent(componentIndex);
    }

    public void SetCheck(bool val)
    {
        isChecked = val;
        checkMark.color = new Color(255, 255, 255, isChecked ? 1f : 0.0f);
    }

}
