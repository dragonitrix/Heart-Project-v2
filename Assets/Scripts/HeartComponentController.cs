using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class HeartComponentController : MonoBehaviour
{
    public bool enablePosControl;
    public bool enableBoneControl;

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {

    }

    public virtual void SetCase(string caseID)
    {

    }

    public virtual void Hide()
    {
        
    }

    public virtual void Show()
    {

    }

    public virtual Vector3 GetPivotPosition()
    {
        return Vector3.zero;
    }

    public virtual Transform GetControlledObj()
    {
        return transform;
    }

    public virtual List<Transform> GetBones()
    {
        return new List<Transform>();
    }

}
