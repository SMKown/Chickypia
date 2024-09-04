using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public static PlayerAbility instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public bool attackMode = false;
    public bool shouldTurn = false;
    public bool shouldAttack = false;

    [SerializeField] private float rotSpeed;
    private Vector3 destinationPoint;

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (attackMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shouldAttack = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100F))
                {
                    destinationPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    shouldTurn = true;
                }
            }

            if (shouldTurn)
            {
                Quaternion targetRot = Quaternion.LookRotation(destinationPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);

                float angle = Quaternion.Angle(transform.rotation, targetRot);
                if (angle <= 0)
                {
                    // 공격 코드

                    shouldTurn = false;
                    shouldAttack = false;
                }
            }
        }
    }
}
