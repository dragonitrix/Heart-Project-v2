using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PosController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform obj;
    RectTransform rectTransform;

    public List<RectTransform> bindedTransforms = new List<RectTransform>();
    List<Vector2> bindedTransformsPos = new List<Vector2>();

    public CanvasGroup kill_button;

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

        bindedTransformsPos.Clear();

        for (int i = 0; i < bindedTransforms.Count; i++)
        {
            bindedTransformsPos.Add(bindedTransforms[i].anchoredPosition - transformOriginPos);
        }

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


        for (int i = 0; i < bindedTransforms.Count; i++)
        {
            bindedTransforms[i].anchoredPosition = rectTransform.anchoredPosition + bindedTransformsPos[i];
        }


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

    public void BindTransforms(List<RectTransform> transforms)
    {
        bindedTransforms.AddRange(transforms);
    }

    public void OnKillClicked()
    {
        Destroy(obj.gameObject);
        foreach (Transform t in bindedTransforms)
        {
            Destroy(t.gameObject);
        }

        Destroy(gameObject);

    }

    public void EnableKillButton()
    {
        kill_button.alpha = 1;
        kill_button.interactable = true;
        kill_button.blocksRaycasts = true;
    }
}
