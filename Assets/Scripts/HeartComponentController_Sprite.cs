using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class HeartComponentController_Sprite : HeartComponentController
{
    public CaseSpriteDatas caseSpriteDatas;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void SetCase(string caseID)
    {
        var sprite = caseSpriteDatas.GetSpriteDataByID(caseID);
        if (sprite) spriteRenderer.sprite = sprite;
    }

    public override void Hide()
    {
        spriteRenderer.enabled = false;
    }

    public override void Show()
    {
        spriteRenderer.enabled = true;
    }
}
