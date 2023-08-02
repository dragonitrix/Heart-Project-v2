using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public class BoneController : MonoBehaviour
{
    public Transform bone;
    RectTransform rectTransform;
    public RectTransform scale_rt;

    public float scale_min = 0.2f;
    public float scale_max = 2f;

    public float scale_value = 1f;

    float scale_rt_origin = 1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        scale_rt_origin = scale_rt.sizeDelta.x / 2;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isHandlerClick && (Input.GetMouseButton(0)))
        {
            var mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scale_rt, mousePos, Camera.main, out Vector2 localPoint);
            //Debug.Log((Vector2)scale_rt.position - localPoint);
            var delta = (Vector2)scale_rt.position - localPoint + mouseOffset;

            var min = Mathf.Abs(Mathf.Min(delta.x, delta.y));

            min = Mathf.Clamp(min, scale_min * scale_rt_origin, scale_max * scale_rt_origin);

            scale_value = min / scale_rt_origin;

            UpdateBoneScale();

            scale_rt.sizeDelta = new Vector2(min * 2, min * 2);
        }
        if (isHandlerClick && (Input.GetMouseButtonUp(0)))
        {
            isHandlerClick = false;
        }
    }

    public void UpdateBoneScale()
    {
        if (bone == null)
        {
            return;
        }

        bone.localScale = new Vector3(1, 1 * scale_value, 1);

        //Debug.Log("scale_value: " + scale_value);
    }


    bool isHandlerClick;
    Vector2 mouseOrigin, mouseOffset;

    public void OnConerClick()
    {
        isHandlerClick = true;

        var mousePos = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(scale_rt, mousePos, Camera.main, out Vector2 localPoint);
        mouseOrigin = localPoint;
        mouseOffset = Vector2.zero;
    }

    public void SetBone(Transform bone)
    {
        this.bone = bone;

        //Debug.Log("rt.position: " + rt.position);
        //Debug.Log("bone.position: " + bone.position);

        rectTransform.anchoredPosition = WorldToCanvasSpace(bone.position);

        scale_rt.sizeDelta = new Vector2(scale_rt_origin * 2 * bone.localScale.y, scale_rt_origin * 2 * bone.localScale.y);

        scale_rt_origin = scale_rt.sizeDelta.x / 2;
    }
    public Vector3 WorldToCanvasSpace(Vector3 pos)
    {
        var cam = Camera.main;
        Vector3 screenPos = cam.WorldToScreenPoint(pos);
        return screenPos;
    }
    public Vector3 ScreenToWorldSpace(Vector3 pos)
    {
        var cam = Camera.main;
        var point = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cam.nearClipPlane));
        return point;
    }
}
