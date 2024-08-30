using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    protected GameObject target;
    protected Vector3 FixedPos;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        FollowCamera();
    }

    virtual protected void FollowCamera()
    {
        FixedPos = new Vector3(target.transform.position.x, 12F, target.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, FixedPos, 0.5F);
    }
}
