using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    
    private Vector3 destinationPoint;
    private bool rotating = false;

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (PlayerInfo.Instance.attackMode && PlayerInfo.Instance.isGround)
        {
            if (Input.GetMouseButtonDown(0) && !PlayerInfo.Instance.attacking)
            {
                PlayerInfo.Instance.attacking = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100F))
                {
                    destinationPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    rotating = true;
                }
            }

            if (rotating)
            {
                Quaternion targetRot = Quaternion.LookRotation(destinationPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);

                float angle = Quaternion.Angle(transform.rotation, targetRot);
                if (angle <= 0)
                {
                    // 공격 코드

                    rotating = false;
                    PlayerInfo.Instance.attacking = false;
                }
            }
        }
    }
}
