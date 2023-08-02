using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "HeartComponent/HeartComponentData")]
public class HeartComponentData : ScriptableObject
{
    public HeartComponentType type;
    public string componentName;
    public List<CaseDetail> cases;
    public bool boneAdjustable = false;
    public bool transformAdjustable = false;
}

[Serializable]
public class CaseDetail
{
    public bool enabled = true;
    public CaseType caseType;
    public string caseName;
    public string displayName;
    public string caseID;

    public string RefineCaseName()
    {
        var names = caseName.Split('_').ToList();

        names.RemoveRange(0, 2);

        var refineName = "";

        for (int i = 0; i < names.Count; i++)
        {
            refineName += names[i];
            if (i != names.Count - 1) refineName += " ";

        }
        return refineName;
    }

}


// data
public enum HeartComponentType
{
    _01_ApexHeart,
    _02_SVC,
    _03_IVC,
    _04_RA,
    _05_TV,
    _06_RV,
    _07_Pul_Valve,
    _08_PA,
    _09_Pul_Vein,
    _10_LA,
    _11_MV,
    _12_LV,
    _13_AV,
    _14_Aorta
}

public enum CaseType
{
    _NORMAL,
    _ADDITIVE
}