using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{

    [SerializeField] private Vector3 direction = new Vector3(0.2f, 1f, 0f);
    private bool done = true;


    private void Start()
    {
        StartCoroutine(TestShot());
    }

    private void FixedUpdate()
    {
        if (!done)
        {
            transform.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
            done = true;
        }
    }
    
    IEnumerator TestShot()
    {
        for (;;)
        {
            direction = new Vector3(Random.Range(-3f, 3f), Random.Range(-2f, -5f), Random.Range(0.5f, 4f));
            done = false;
            
            transform.GetComponent<Rigidbody>().position = Vector3.zero;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            yield return new WaitForSeconds(3f);
        }
    }
}
