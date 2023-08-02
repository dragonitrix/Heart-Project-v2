using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "HeartComponent/CaseSpriteDatas")]
public class CaseSpriteDatas : ScriptableObject
{
    public List<CaseSpriteData> caseSpriteDatas = new List<CaseSpriteData>();
    public Sprite GetSpriteDataByID(string id)
    {
        for (int i = 0; i < caseSpriteDatas.Count; i++)
        {
            if (caseSpriteDatas[i].id == id) return caseSpriteDatas[i].sprite;
        }
        return null;
    }
}

[Serializable]
public class CaseSpriteData
{
    public string id;
    public Sprite sprite;
}