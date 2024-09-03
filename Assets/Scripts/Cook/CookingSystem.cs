using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    private static bool mIsCooking = false;
    public static bool isCooking
    {
        get { return mIsCooking; }
    }

    public ParticleSystem making;

    public bool canCook;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCook = true;
            if(canCook)
            {
                Cook(other);
            }
        }
    }

    void Cook(Collider other)
    {
        making = other.gameObject.GetComponent<ParticleSystem>();

        if (making != null)
        {
            making.Play();
        }
        else
        {
            Debug.Log("No");
        }
    }
}
