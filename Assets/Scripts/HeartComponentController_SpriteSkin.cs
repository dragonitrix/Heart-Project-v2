using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class HeartComponentController_SpriteSkin : HeartComponentController
{

    public SpriteLibrary spriteLibrary;
    public SpriteResolver spriteResolver;
    private SpriteLibraryAsset libraryAsset;

    private SpriteRenderer spriteRenderer;

    public Transform controlledObjOverried;
    public Transform pivotOverried;

    public List<Transform> bones = new List<Transform>();

    void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        libraryAsset = spriteLibrary.spriteLibraryAsset;

        spriteResolver = transform.GetChild(0).GetComponent<SpriteResolver>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override void SetCase(string caseID)
    {
        var catagoryName = libraryAsset.GetCategoryNames().ToArray()[0];
        string[] labels = libraryAsset.GetCategoryLabelNames(catagoryName).ToArray();
        string label = "";


        foreach (string labelName in labels)
        {
            if (labelName.Contains(caseID))
            {
                label = labelName;
            }
        }

        if (!string.IsNullOrEmpty(label))
        {
            spriteResolver.SetCategoryAndLabel(catagoryName, label);
        }
        else
        {
            Debug.Log($"no {label} label found on {catagoryName}");
        }

    }

    public override void Hide()
    {
        spriteRenderer.enabled = false;
    }

    public override void Show()
    {
        spriteRenderer.enabled = true;
    }

    public override Vector3 GetPivotPosition()
    {
        if (pivotOverried)
        {
            return pivotOverried.transform.position;
        }
        else
        {
            return spriteRenderer.transform.position;
        }
    }

    public override Transform GetControlledObj()
    {
        if (controlledObjOverried)
        {
            return controlledObjOverried.transform;
        }
        else
        {

            return base.GetControlledObj();
        }
    }

    public override List<Transform> GetBones()
    {
        return bones;
    }
}
