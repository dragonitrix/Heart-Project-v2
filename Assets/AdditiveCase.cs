using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveCase : MonoBehaviour
{
    public string id;
    public Transform pivot;

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAdditiveCase(string id)
    {
        var sprite = GameManager.instance.heartSprites[id];
        spriteRenderer.sprite = sprite;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector3 GetPivotPosition()
    {
        return pivot ? pivot.position : transform.position;
    }
    public virtual Transform GetControlledObj()
    {
        return transform;
    }
}
