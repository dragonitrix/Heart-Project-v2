using FreeDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{


    public static UIController instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        mainCanvas = GetComponent<RectTransform>();

    }

    RectTransform mainCanvas;

    [Header("Control UI")]
    public Button posControl_button;
    public Button boneControl_button;

    [Header("Pen UI")]
    public RectTransform pen_cursor;
    public CanvasGroup pen_panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Drawable.drawable.isDrawing)
        {
            var mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, mousePos, Camera.main, out Vector2 localPoint);
            //Debug.Log((Vector2)scale_rt.position - localPoint);
            pen_cursor.localPosition = localPoint;
        }
    }

    public void OnPosControlClicked()
    {
        if (!GameManager.instance.isPosControlling) GameManager.instance.InitPosControl();
        else GameManager.instance.StopAllUIControl();
    }

    public void OnBoneControlClicked()
    {
        if (!GameManager.instance.isBoneControlling) GameManager.instance.InitBoneControl();
        else GameManager.instance.StopAllUIControl();
    }


    public void UpdateButtonStatus(bool posControl,bool boneControl)
    {
        posControl_button.interactable = posControl;
        boneControl_button.interactable = boneControl;
    }


    //pen controller
    public void OnPenClick()
    {
        Drawable.drawable.isDrawing = !Drawable.drawable.isDrawing;

        SetMarkerWidth(10);

        if (Drawable.drawable.isDrawing)
        {
            pen_panel.alpha = 1;
            pen_panel.interactable = true;
            pen_panel.blocksRaycasts = true;

            pen_cursor.GetComponent<CanvasGroup>().alpha = 1;
            pen_cursor.GetComponent<CanvasGroup>().interactable = true;
            pen_cursor.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            pen_panel.alpha = 0;
            pen_panel.interactable = false;
            pen_panel.blocksRaycasts = false;

            pen_cursor.GetComponent<CanvasGroup>().alpha = 0;
            pen_cursor.GetComponent<CanvasGroup>().interactable = false;
            pen_cursor.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }


    public void SetMarkerColour(Color new_color)
    {
        Drawable.Pen_Colour = new_color;
    }
    // new_width is radius in pixels
    public void SetMarkerWidth(int new_width)
    {
        Drawable.Pen_Width = new_width;

        var offset = 10;

        pen_cursor.sizeDelta = new Vector2(new_width * 2 + offset, new_width * 2 + offset);

    }
    public void SetMarkerWidth(float new_width)
    {
        SetMarkerWidth((int)new_width);
    }


    public void SetMarkerWhite()
    {
        Color c = Color.white;
        c.a = 1;
        SetMarkerColour(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerBlack()
    {
        Color c = Color.black;
        c.a = 1;
        SetMarkerColour(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerGray()
    {
        Color c = new Color(180f / 255f, 180f / 255f, 180f / 255f);
        c.a = 1;
        SetMarkerColour(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetEraser()
    {
        SetMarkerColour(new Color(255f, 255f, 255f, 0f));
    }


    public void ResetCanvas()
    {
        Drawable.drawable.ResetCanvas();
    }

}
