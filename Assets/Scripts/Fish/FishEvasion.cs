using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishEvasion : MonoBehaviour
{
    [SerializeField] private RectTransform maxHeight;
    [SerializeField] private RectTransform minHeight;
    
    [Range(0, 5f)]public float moveSpeed;
    public float maxWaitTime, minWaitTime;
    
    private Vector3 currentDestination;

    private bool waiting = false;

    private void Start()
    {
        currentDestination = RandomDestination();
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, currentDestination, moveSpeed  * Time.deltaTime); 
        
        if (Vector3.Distance(transform.position, currentDestination) <= 1f && !waiting) 
        { 
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait() 
    {
        waiting = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        currentDestination = RandomDestination();
        waiting = false;
    }

    private Vector3 RandomDestination() 
    {
        var rectT = GetComponent<RectTransform>();
        var maxUp = maxHeight.position.y - rectT.sizeDelta.y/2;
        var maxDown = minHeight.position.y + rectT.sizeDelta.y/2;
        var newHeight = Random.Range(maxUp, maxDown);

        return new Vector3(transform.position.x, newHeight, transform.position.z);
    }
}
