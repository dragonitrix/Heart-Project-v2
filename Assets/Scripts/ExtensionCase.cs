using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class ExtensionCase : AdditiveCase
{

    public SpriteLibrary spriteLibrary;
    public SpriteResolver spriteResolver;
    private SpriteLibraryAsset libraryAsset;

    public List<Transform> controlledObjsOverried = new List<Transform>();

    public List<Transform> bones = new List<Transform>();

    protected override void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        libraryAsset = spriteLibrary.spriteLibraryAsset;

        spriteResolver = transform.GetChild(0).GetComponent<SpriteResolver>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override void SetAdditiveCase(string caseID)
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

    public override Vector3 GetPivotPosition()
    {
        return pivot ? pivot.position : transform.position;
    }
    public override List<Transform> GetControlledObjs()
    {
        if (controlledObjsOverried.Count > 0)
        {
            return controlledObjsOverried;
        }
        else
        {
            return new List<Transform> { transform };
        }
    }
    public List<Transform> GetBones()
    {
        return bones;
    }
}
