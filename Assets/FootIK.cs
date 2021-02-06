using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    public Transform foot;
    private Transform originalPos;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = foot;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var ray = new Ray(foot.transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hitInfo;
        if(Physics.SphereCast(ray, 0.05f, out hitInfo, 0.50f)) 
            foot.position = hitInfo.point + Vector3.up * 0.05f;
        else 
            foot.position = originalPos.position;
    }
}
