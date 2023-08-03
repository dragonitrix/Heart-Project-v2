using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveCase : MonoBehaviour
{
    public string id;
    public Transform pivot;

    public SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void SetAdditiveCase(string id)
    {
        var sprite = GameManager.instance.heartSprites[id];
        spriteRenderer.sprite = sprite;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
    public virtual Vector3 GetPivotPosition()
    {
        return pivot ? pivot.position : transform.position;
    }
    public virtual List<Transform> GetControlledObjs()
    {
        return new List<Transform> { transform };
    }
}
