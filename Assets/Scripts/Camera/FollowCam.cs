using UnityEngine;

public class FollowCam : MonoBehaviour
{
    protected GameObject target;
    protected Vector3 FixedPos;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        FollowCamera();
    }

    virtual protected void FollowCamera()
    {
        FixedPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.position = FixedPos;
    }
}
