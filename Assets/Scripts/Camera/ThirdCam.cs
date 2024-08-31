using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : FollowCam
{
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [SerializeField] private float offsetZ;
    [SerializeField] private float DelayTime;

    override protected void FollowCamera()
    {
        Vector3 FixedPos = new Vector3(target.transform.position.x + offsetX,
            transform.position.y + offsetY, target.transform.position.z + offsetZ);
        transform.position = Vector3.Lerp(transform.position, FixedPos, DelayTime * Time.deltaTime);
    }
}
