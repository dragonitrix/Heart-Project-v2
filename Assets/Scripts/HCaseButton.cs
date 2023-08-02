using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HCaseButton : MonoBehaviour
{
    public bool isSelected = false;
    public bool isAdded = false;
    public bool isAdditive = false;
    public string caseName;
    public int caseIndex;

    [Header("obj ref")]
    public TextMeshProUGUI caseNameText;
    public Image caseImage;
    public Image selectedPointer;
    public Image addedPointer;
    public Button caseButton;



    public void Start()
    {
        caseButton.onClick.AddListener(() => { OnButtonClick(); });

        //addedPointer.color = new Color(1, 1, 1, 0.0f);

    }

    public void EnableAdditive()
    {
        isAdditive = true;
        isAdded = false;
        addedPointer.color = new Color(1, 1, 1, 0.2f);
    }

    public void ToggleAdditive()
    {
        SetAdditive(!isAdded);
    }
    public void SetAdditive(bool val)
    {
        isAdded = val;
        addedPointer.color = new Color(1, 1, 1, isAdded ? 1f : 0.2f);
    }

    public void OnButtonClick()
    {
        GameManager.instance.OnSetAbnormalCaseClicked(this,caseIndex);
    }

    public void SetData(string name, Sprite sprite, int index)
    {
        caseName = name;
        caseNameText.text = name;
        caseImage.sprite = sprite;
        SetSizeAutoFit();
        caseIndex = index;
    }

    public void Select(bool val)
    {
        isSelected = val;
        if (isSelected)
        {
            selectedPointer.color = new Color(1, 1, 1, 1f);
        }
        else
        {
            selectedPointer.color = new Color(1, 1, 1, 0f);
        }
    }
    public void SetSizeAutoFit()
    {
        var sprite = caseImage.sprite;
        if (sprite.texture.width > sprite.texture.height)
        {
            SetSizeFromWidth();
        }
        else
        {
            SetSizeFromHeight();
        }
    }

    public void SetSizeFromWidth()
    {
        SetSizeFromWidth(caseImage.rectTransform.sizeDelta.x);
    }
    public void SetSizeFromWidth(float minWidth)
    {
        float ratio = (float)caseImage.mainTexture.width / (float)caseImage.mainTexture.height;
        var sizeDelta = new Vector2(
            minWidth,
            minWidth / ratio
            );
        caseImage.rectTransform.sizeDelta = sizeDelta;
    }

    public void SetSizeFromHeight()
    {
        SetSizeFromHeight(caseImage.rectTransform.sizeDelta.y);
    }
    public void SetSizeFromHeight(float minHeight)
    {
        float ratio = (float)caseImage.mainTexture.width / (float)caseImage.mainTexture.height;
        var sizeDelta = new Vector2(
            minHeight * ratio,
            minHeight
            );
        caseImage.rectTransform.sizeDelta = sizeDelta;
    }

}
