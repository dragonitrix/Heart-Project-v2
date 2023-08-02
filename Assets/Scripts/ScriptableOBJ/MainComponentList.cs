using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HeartComponent/MainComponentList")]
public class MainComponentList : ScriptableObject
{
    public List<HeartComponentData> heartComponentList;

    public List<CaseDetail> GetCasesDetail(HeartComponentType type)
    {
        return heartComponentList[(int)type].cases;
    }

}
