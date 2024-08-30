using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : FollowCam
{
    [SerializeField] private float offsetZ;
    [SerializeField] private float DelayTime;

    override protected void FollowCamera()
    {
        Vector3 FixedPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z + offsetZ);
        transform.position = Vector3.Lerp(transform.position, FixedPos, DelayTime);
    }
}
