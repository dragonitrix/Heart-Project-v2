using FreeDraw;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public void OnSetAbnormalCaseClicked(HCaseButton btn, int index)
    {
        if (selectedHComponentIndex == 99)
        {
            CreateExtension(index);
        }
        else
        {
            SetAbnormalCase(selectedHComponentIndex, index, true);
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

        if (selectedHComponentIndex == 99)
        {
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
        }
        else
        {
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
        }
        isPosControlling = true;

    }

    [ContextMenu("Start Bone Control")]
    public void InitBoneControl()
    {
        StopAllUIControl();
        var currentHComponent = GetCurrentControllers();
        var mainComponent = currentHComponent[0];

        if (!mainComponent.enableBoneControl) { return; }

        isBoneControlling = true;


        var bones = mainComponent.GetBones();

        foreach (var bone in bones)
        {
            var clone = Instantiate(boneController_prefab, boneController_rect);
            var boneControl = clone.GetComponent<BoneController>();
            //boneControl.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(mainComponent.GetPivotPosition());
            boneControl.SetBone(bone);
        }
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


}
