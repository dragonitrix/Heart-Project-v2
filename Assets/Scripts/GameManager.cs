using FreeDraw;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ImageDownloader(string str, string fn);

    public static GameManager instance;
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
    }

    [Header("DATA")]
    public MainComponentList mainComponentList;
    public List<Transform> additiveCaseGroups;
    public Transform extensionGroup;

    [Header("Prefabs")]
    public GameObject hComponent_prefab;
    public GameObject hCase_prefab;
    public GameObject additiveCase_prefab;
    public List<GameObject> additiveCase_prefabs = new List<GameObject>();
    public GameObject extension_prefab;

    [Header("Linking list")]
    public List<HeartControllerList> heartControllerLists;

    [Header("Component UI")]
    public RectTransform hComponent_rect;

    [Header("Case UI")]
    public RectTransform hCase_rect;

    public Dictionary<string, Sprite> heartSprites = new Dictionary<string, Sprite>();

    public List<AdditiveCase> additiveCases = new List<AdditiveCase>();
    public List<ExtensionCase> extensionCases = new List<ExtensionCase>();

    public List<List<bool>> ActiveCases = new List<List<bool>>();
    public List<int> currentActiveCaseIndexes = new List<int>();

    public List<HComponentButton> hComponentButtons = new List<HComponentButton>();
    public List<HCaseButton> hCaseButtons = new List<HCaseButton>();

    [Header("Controller UI Prefab")]
    public GameObject posController_prefab;
    public GameObject boneController_prefab;
    [Header("Controller UI")]
    public RectTransform posController_rect;
    public RectTransform boneController_rect;
    [Header("Export")]
    public CanvasScreenShot canvasScreenShot;
    public Canvas exportCanvas;
    public RectTransform exportPanel;
    public Button exportButton;
    public RawImage exportedRawImage;

    public RULE rule = RULE._00_NormalHeart;

    public enum RULE
    {
        _00_NormalHeart, _01_Dextrocardia_1, _02_Dextrocardia_2
    }

    [Header("RULE1_portion rotate")]
    public List<Transform> rule_01_flipLists = new List<Transform>();

    [Header("RULE2_flip")]
    public List<Transform> rule_02_flipLists = new List<Transform>();

    [Serializable]
    public class Rule_01_position
    {
        public Transform obj;
        public Transform originTransform;
        public Transform ruleTransform;
    }

    [Serializable]
    public class HeartControllerList
    {
        public List<HeartComponentController> controllerList;

        public void SetCaseAll(string caseID)
        {
            foreach (HeartComponentController controller in controllerList)
            {
                controller.SetCase(caseID);
            }
        }

        public void HideAll()
        {
            foreach (HeartComponentController controller in controllerList)
            {
                controller.Hide();
            }
        }
        public void ShowAll()
        {
            foreach (HeartComponentController controller in controllerList)
            {
                controller.Show();
            }
        }

    }

    public int selectedHComponentIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        //init export callback
        CanvasScreenShot.OnPictureTaken = delegate { }; //clear old event
        CanvasScreenShot.OnPictureTaken += OnPictureTaken;

        //Debug.Log("Fetch all cases");
        //
        //for (int i = 0; i < mainComponentList.heartComponentList.Count; i++)
        //{
        //    var heartComponent = mainComponentList.heartComponentList[i];
        //
        //    var text = heartComponent.type.ToString() + "\n";
        //
        //    for (int j = 0; j < heartComponent.cases.Count; j++)
        //    {
        //        var componentCase = heartComponent.cases[j];
        //
        //        if (componentCase.enabled)
        //        {
        //            //Debug.Log(componentCase.caseName);
        //            text += $"\t{RefineCaseName(componentCase.caseName)}\n";
        //        }
        //
        //    }
        //
        //    Debug.Log(text);
        //}

        //fetch heart image
        var heartSprites = Resources.LoadAll("heart sprites", typeof(Sprite)).Cast<Sprite>();

        foreach (var item in heartSprites)
        {
            var id = NameToID(item.name);
            this.heartSprites.Add(id, item);

        }

        initHComponentPanel();

        foreach (var hComponentData in mainComponentList.heartComponentList)
        {
            var activeCase = new List<bool>();
            foreach (var hCase in hComponentData.cases)
            {
                //if (hCase.enabled)
                //{
                //    activeCase.Add(false);
                //}
                activeCase.Add(false);
            }
            ActiveCases.Add(activeCase);
        }

        for (int i = 0; i < 14; i++)
        {
            currentActiveCaseIndexes.Add(-1);
        }

        for (int i = 0; i < 14; i++)
        {
            SetAbnormalCase(i, -1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRule(RULE rule)
    {
        //Debug.Log("set rule");

        //exit
        switch (this.rule)
        {
            case RULE._00_NormalHeart:
                break;
            case RULE._01_Dextrocardia_1:
                break;
            case RULE._02_Dextrocardia_2:
                foreach (var t in rule_02_flipLists)
                {
                    t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
                }
                break;
            default:
                break;
        }
        this.rule = rule;
        //enter
        switch (this.rule)
        {
            case RULE._00_NormalHeart:
                break;
            case RULE._01_Dextrocardia_1:

                break;
            case RULE._02_Dextrocardia_2:
                foreach (var t in rule_02_flipLists)
                {
                    t.localScale = new Vector3(t.localScale.x * -1, t.localScale.y, t.localScale.z);
                }

                break;
            default:
                break;
        }
    }

    public void OnSetAbnormalCaseClicked(HCaseButton btn, int index)
    {
        switch (selectedHComponentIndex)
        {
            case 0:
                SetRule((RULE)index);
                break;
            case 99:
                CreateExtension(index);
                break;
            default:
                SetAbnormalCase(selectedHComponentIndex, index, true);
                break;
        }

    }

    public void SetAbnormalCase(int hIndex, int index, bool setUI = false)
    {

        //Debug.Log($"set {hIndex} to abnormal case {index}");

        var last_CaseIndex = currentActiveCaseIndexes[hIndex];


        if (index == -1)
        {
            switch (hIndex)
            {
                case 3:
                case 5:
                case 9:
                case 11:
                    //set to 0 instead
                    SetAbnormalCase(hIndex, 0);
                    break;
                default:
                    //hidden component instead
                    heartControllerLists[hIndex].HideAll();
                    break;
            }
            return;
        }

        var caseDetail = mainComponentList.heartComponentList[hIndex].cases[index];
        var caseID = caseDetail.caseID;



        switch (caseDetail.caseType)
        {
            case CaseType._NORMAL:
                if (last_CaseIndex == -1)
                {
                    heartControllerLists[hIndex].ShowAll();
                    hComponentButtons[hIndex].SetCheck(true);
                }

                heartControllerLists[hIndex].SetCaseAll(caseID);
                //set button
                if (setUI)
                {
                    var lastBtn = TryGetCaseButton(last_CaseIndex);
                    if (lastBtn) lastBtn.Select(false);
                    var currentBtn = TryGetCaseButton(index);
                    if (currentBtn) currentBtn.Select(true);
                }

                currentActiveCaseIndexes[hIndex] = index;

                if (last_CaseIndex != -1) ActiveCases[hIndex][last_CaseIndex] = false;
                ActiveCases[hIndex][index] = true;

                break;
            case CaseType._ADDITIVE:
                CreateAdditiveCase(hIndex, caseDetail);
                //set button
                if (setUI)
                {
                    var currentAdditiveBtn = TryGetCaseButton(index);
                    if (currentAdditiveBtn) currentAdditiveBtn.ToggleAdditive();
                }

                ActiveCases[hIndex][index] = !ActiveCases[hIndex][index];
                break;
            default:
                break;
        }

        UpdateAllUIControl();
    }


    public void CreateAdditiveCase(int hIndex, CaseDetail caseDetail)
    {
        var caseID = caseDetail.caseID;

        AdditiveCase dupe = null;
        var dupeResult = false;

        for (int i = 0; i < additiveCases.Count; i++)
        {
            var additiveCase = additiveCases[i];

            if (additiveCase.id == caseID)
            {
                dupe = additiveCase;
                dupeResult = true;
                break;
            }
        }

        if (dupeResult)
        {
            additiveCases.Remove(dupe);
            Destroy(dupe.gameObject);

            UpdateAllUIControl();
            return;
        }

        for (int i = 0; i < additiveCase_prefabs.Count; i++)
        {
            var casePrefab = additiveCase_prefabs[i];
            if (casePrefab.name == caseID)
            {
                var clone = Instantiate(casePrefab, additiveCaseGroups[hIndex]);
                var additiveCaseScript = clone.GetComponent<AdditiveCase>();
                additiveCases.Add(additiveCaseScript);
                break;
            }
        }

        UpdateAllUIControl();
    }

    public void CreateExtension(int extensionIndex)
    {
        var clone = Instantiate(extension_prefab, extensionGroup);
        var extensionCaseScript = clone.GetComponent<ExtensionCase>();

        var caseID = "99_01";

        switch (extensionIndex)
        {
            case 0:
                caseID = "99_01";
                break;
            case 1:
                caseID = "99_02";
                break;
            case 2:
                caseID = "99_03";
                break;
        }

        extensionCaseScript.SetAdditiveCase(caseID);

        extensionCases.Add(extensionCaseScript);

        UpdateAllUIControl();

    }

    public void SelectHeartComponent(int index)
    {
        StopAllUIControl();
        selectedHComponentIndex = index;

        if (selectedHComponentIndex == 99)
        {
            initHExtensionPanel();
        }
        else
        {
            var caseDetail = mainComponentList.heartComponentList[index].cases;
            initHCasePanel(caseDetail);
        }

        switch (selectedHComponentIndex)
        {
            case 0:
                break;
            case 99:
                UIController.instance.UpdateButtonStatus(true, true);
                break;
            default:
                var currentControl = GetCurrentControllers()[0];
                UIController.instance.UpdateButtonStatus(currentControl.enablePosControl, currentControl.enableBoneControl);
                break;
        }

    }

    public void initHComponentPanel()
    {
        foreach (Transform t in hComponent_rect)
        {
            Destroy(t.gameObject);
        }
        hComponentButtons.Clear();

        for (int i = 0; i < mainComponentList.heartComponentList.Count; i++)
        {
            var heartComponent = mainComponentList.heartComponentList[i];
            var h_btn = Instantiate(hComponent_prefab, hComponent_rect);

            var hComponentButton = h_btn.GetComponent<HComponentButton>();

            string name = heartComponent.componentName;
            hComponentButton.SetData(name, i);
            hComponentButtons.Add(hComponentButton);
        }

        //extra component
        var etc_btn = Instantiate(hComponent_prefab, hComponent_rect);

        var etcButton = etc_btn.GetComponent<HComponentButton>();

        etcButton.SetData("etc...", 99);
        hComponentButtons.Add(etcButton);

    }

    public void initHCasePanel(List<CaseDetail> caseDatails)
    {
        foreach (Transform t in hCase_rect)
        {
            Destroy(t.gameObject);
        }
        hCaseButtons.Clear();
        var count = 0;

        for (int i = 0; i < caseDatails.Count; i++)
        {
            var hCase = caseDatails[i];

            if (!hCase.enabled) continue;

            heartSprites.TryGetValue(hCase.caseID, out Sprite sprite);

            if (sprite == null) continue;

            var clone = Instantiate(hCase_prefab, hCase_rect);
            var hCaseButton = clone.GetComponent<HCaseButton>();
            hCaseButton.SetData(hCase.RefineCaseName(), heartSprites[hCase.caseID], i);
            if (hCase.caseType == CaseType._ADDITIVE)
            {
                hCaseButton.EnableAdditive();
            }
            count++;

            //set UI active
            switch (hCase.caseType)
            {
                case CaseType._NORMAL:
                    hCaseButton.Select(ActiveCases[selectedHComponentIndex][i]);
                    break;
                case CaseType._ADDITIVE:
                    hCaseButton.SetAdditive(ActiveCases[selectedHComponentIndex][i]);
                    break;
                default:
                    break;
            }

            hCaseButtons.Add(hCaseButton);
        }

        //resize
        hCase_rect.sizeDelta = new Vector2(
            hCase_rect.sizeDelta.x,
            count * (292 + 15)
            );
    }

    public void initHExtensionPanel()
    {
        foreach (Transform t in hCase_rect)
        {
            Destroy(t.gameObject);
        }
        hCaseButtons.Clear();

        var clone1 = Instantiate(hCase_prefab, hCase_rect);
        var hCaseButton1 = clone1.GetComponent<HCaseButton>();
        hCaseButton1.SetData("stitch", heartSprites["99_01"], 0);
        hCaseButtons.Add(hCaseButton1);

        var clone2 = Instantiate(hCase_prefab, hCase_rect);
        var hCaseButton2 = clone2.GetComponent<HCaseButton>();
        hCaseButton2.SetData("tube", heartSprites["99_02"], 1);
        hCaseButtons.Add(hCaseButton2);

        var clone3 = Instantiate(hCase_prefab, hCase_rect);
        var hCaseButton3 = clone3.GetComponent<HCaseButton>();
        hCaseButton3.SetData("vein", heartSprites["99_03"], 2);
        hCaseButtons.Add(hCaseButton3);

        //resize
        hCase_rect.sizeDelta = new Vector2(
            hCase_rect.sizeDelta.x,
            3 * (292 + 15)
            );
    }


    public List<HeartComponentController> GetCurrentControllers()
    {
        return heartControllerLists[selectedHComponentIndex].controllerList;
    }

    public List<HeartComponentController> GetControllersByType(HeartComponentType type)
    {
        return heartControllerLists[(int)type].controllerList;
    }

    public string NameToID(string name)
    {
        var names = name.Split('_');

        return $"{names[0]}_{names[1]}";

    }

    public HCaseButton TryGetCaseButton(int index)
    {
        if (index < 0) return null;
        for (int i = 0; i < hCaseButtons.Count; i++)
        {
            if (hCaseButtons[i].caseIndex == index)
            {
                return hCaseButtons[i];

            }
        }
        return null;
    }


    public bool isPosControlling = false;
    public bool isBoneControlling = false;

    //Heart Component Controller

    [ContextMenu("Start Pos Control")]
    public void InitPosControl()
    {
        StopAllUIControl();

        switch (selectedHComponentIndex)
        {
            case 99:
                for (int i = 0; i < extensionCases.Count; i++)
                {

                    var extensionCase = extensionCases[i];

                    var mainControlClone = Instantiate(posController_prefab, posController_rect);
                    var mainPosControl = mainControlClone.GetComponent<PosController>();
                    mainPosControl.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(extensionCase.GetPivotPosition());
                    mainPosControl.SetPosController(extensionCase.GetControlledObjs()[0]);

                    var sub1Clone = Instantiate(posController_prefab, posController_rect);
                    var sub1Control = sub1Clone.GetComponent<PosController>();
                    sub1Control.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(extensionCase.GetControlledObjs()[1].position);
                    sub1Control.SetPosController(extensionCase.GetControlledObjs()[1]);

                    var sub2Clone = Instantiate(posController_prefab, posController_rect);
                    var sub2Control = sub2Clone.GetComponent<PosController>();
                    sub2Control.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(extensionCase.GetControlledObjs()[2].position);
                    sub2Control.SetPosController(extensionCase.GetControlledObjs()[2]);

                    mainPosControl.BindTransforms(new List<RectTransform> { sub1Control.GetComponent<RectTransform>(), sub2Control.GetComponent<RectTransform>() });
                    mainPosControl.EnableKillButton();
                }
                break;
            default:

                var currentHComponent = GetCurrentControllers();
                var mainComponent = currentHComponent[0];

                if (!mainComponent.enablePosControl) { return; }

                var clone = Instantiate(posController_prefab, posController_rect);
                var posControl = clone.GetComponent<PosController>();

                posControl.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(mainComponent.GetPivotPosition());
                posControl.SetPosController(mainComponent.GetControlledObj());

                // set controller for additive case

                for (int i = 0; i < additiveCaseGroups[selectedHComponentIndex].transform.childCount; i++)
                {
                    var additiveTransform = additiveCaseGroups[selectedHComponentIndex].transform.GetChild(i);
                    var additiveCaseController = additiveTransform.GetComponent<AdditiveCase>();
                    var additiveClone = Instantiate(posController_prefab, posController_rect);
                    var additivePosControl = additiveClone.GetComponent<PosController>();

                    additivePosControl.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(additiveCaseController.GetPivotPosition());
                    additivePosControl.SetPosController(additiveCaseController.GetControlledObjs()[0]);
                }
                break;
        }
        isPosControlling = true;

    }

    [ContextMenu("Start Bone Control")]
    public void InitBoneControl()
    {
        StopAllUIControl();

        switch (selectedHComponentIndex)
        {
            case 99:
                break;
            default:

                var currentHComponent = GetCurrentControllers();
                var mainComponent = currentHComponent[0];

                if (!mainComponent.enableBoneControl) { return; }

                var bones = mainComponent.GetBones();

                foreach (var bone in bones)
                {
                    var clone = Instantiate(boneController_prefab, boneController_rect);
                    var boneControl = clone.GetComponent<BoneController>();
                    //boneControl.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(mainComponent.GetPivotPosition());
                    boneControl.SetBone(bone);
                }
                break;
        }
        isBoneControlling = true;
    }

    public void StopAllUIControl()
    {
        foreach (Transform t in posController_rect)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in boneController_rect)
        {
            Destroy(t.gameObject);
        }

        isPosControlling = false;
        isBoneControlling = false;
    }

    public void UpdateAllUIControl()
    {
        StartCoroutine(_UpdateAllUIControl());
    }

    IEnumerator _UpdateAllUIControl()
    {
        yield return new WaitForEndOfFrame();
        if (isPosControlling) { InitPosControl(); }
        if (isBoneControlling) { InitBoneControl(); }
    }

    //helper
    public Vector3 WorldToCanvasSpace(Vector3 pos)
    {
        var cam = Camera.main;
        Vector3 screenPos = cam.WorldToScreenPoint(pos);
        return screenPos;
    }

    #region(export)
    //public Texture2D ComplieRenderTexture()
    //{
    //    Texture2D tex = new Texture2D(renderTextures[0].width, renderTextures[0].height);
    //    for (int i = 0; i < renderTextures.Count; i++)
    //    {
    //        var rt = renderTextures[i];
    //        //RenderTexture.active = rt;
    //
    //        tex.CombineTexture2D(rt.GetRTPixels());
    //
    //        //tex = ExtensionMethods.CombineTexture2D(tex, rt.GetRTPixels());
    //
    //        //int startX = 0;
    //        //int startY = 0;
    //        //
    //        //for (int x = startX; x < tex.width; x++)
    //        //{
    //        //
    //        //    for (int y = startY; y < tex.height; y++)
    //        //    {
    //        //        Color bgColor = tex.GetPixel(x, y);
    //        //        Color wmColor = rt.GetRTPixels().GetPixel(x, y);
    //        //
    //        //        Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
    //        //
    //        //        tex.SetPixel(x, y, final_color);
    //        //    }
    //        //}
    //        //
    //        //tex.Apply();
    //    }
    //    return tex;
    //}

    //public Texture2D compliedRenderTextures;

    public void OnTakeScreenShot()
    {
        //StartCoroutine(takeScreenShot());
        canvasScreenShot.takeScreenShot(exportCanvas, new Vector2(1080, 1080), SCREENSHOT_TYPE.IMAGE_AND_TEXT, false);
    }

    IEnumerator takeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        //var preferredSize = new Vector2(Screen.height, Screen.height);
        var preferredSize = new Vector2(UIController.instance.mainCanvas.sizeDelta.y, UIController.instance.mainCanvas.sizeDelta.y);

        Texture2D texture2D = exportedRawImage.texture.GetReadableTexture2d();

        Texture2D croppedTexture = new Texture2D((int)preferredSize.x, (int)preferredSize.y);
        croppedTexture.SetPixels(
            texture2D.GetPixels(
                0,
                texture2D.height - (int)preferredSize.y,
                (int)preferredSize.x,
                (int)preferredSize.y
                )
            );
        croppedTexture.Apply();
        ExportImageToDevice(croppedTexture);
        Destroy(croppedTexture);

    }

    void OnPictureTaken(Texture2D texture2D, byte[] pngArray)
    {
        var preferredSize = new Vector2(UIController.instance.mainCanvas.sizeDelta.y, UIController.instance.mainCanvas.sizeDelta.y);

        //Texture2D rts = ComplieRenderTexture();

        //var rts = renderTextures[0].GetRTPixels();

        //texture2D.CombineTexture2D(rts);

        //texture2D = compliedRenderTextures;

        //texture2D = ExtensionMethods.CombineTexture2D(texture2D, rts);


        Texture2D croppedTexture = new Texture2D((int)preferredSize.x, (int)preferredSize.y);
        croppedTexture.SetPixels(
            texture2D.GetPixels(
                0,
                texture2D.height - (int)preferredSize.y,
                (int)preferredSize.x,
                (int)preferredSize.y
                )
            );
        croppedTexture.Apply();
        ExportImageToDevice(croppedTexture);
        Destroy(croppedTexture);


        //exportCanvas.gameObject.SetActive(false);

    }
    void ExportImageToDevice(Texture2D texture2D)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        //iphone
        byte[] imageData = texture2D.EncodeToPNG();
        DateTime dateTime = DateTime.Now;
        var imageFilename = "export_" + dateTime.ToString("yyyyMMddHHmmss");
        Debug.Log("Downloading..." + imageFilename);
        ImageDownloader(System.Convert.ToBase64String(imageData), imageFilename);

#else
        string parentPath = Application.persistentDataPath + "/ExportData";
        if (!Directory.Exists(parentPath))
        {
            Directory.CreateDirectory(parentPath);
        }
        //Convert to png
        byte[] pngBytes = texture2D.EncodeToPNG();
        //Do Something With the Image (Save)
        DateTime dateTime = DateTime.Now;
        var stamp = dateTime.ToString("yyyyMMddHHmmss");
        string path = parentPath + "/" + stamp + ".png";
        System.IO.File.WriteAllBytes(path, pngBytes);
        Debug.Log(path);
        // To avoid memory leaks
        Destroy(texture2D);
#endif
    }

    #endregion

}
