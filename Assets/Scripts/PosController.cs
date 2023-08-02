using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PosController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform obj;
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosController(Transform objToControl)
    {
        obj = objToControl;
    }

    bool isDraging;
    Vector2 transformOriginPos;
    Vector3 objOriginPos;
    Vector3 startMousePos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraging) return;
        isDraging = true;

        transformOriginPos = rectTransform.anchoredPosition;
        objOriginPos = obj.position;
        startMousePos = Input.mousePosition;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraging) return;
        var startWorldPos = ScreenToWorldSpace(startMousePos);
        var currentWorldPos = ScreenToWorldSpace(Input.mousePosition);
        var delta = startWorldPos - currentWorldPos;
        obj.position = objOriginPos - delta;

        var screenDelta = startMousePos - Input.mousePosition;

        rectTransform.anchoredPosition = transformOriginPos - (Vector2)screenDelta;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraging) return;
        isDraging = false;
    }

    public Vector3 ScreenToCanvasSpace(Vector3 pos)
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
