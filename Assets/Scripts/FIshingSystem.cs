using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIshingSystem : MonoBehaviour
{
    private bool isFishing;
    private int randomNumber;

    // list of each fish that can potentially be caught
    public List<GameObject> fishTypes = new List<GameObject>();

    // the currently selected fish out of the list
    public GameObject selectedFish;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnLeftClick()
    {
        if (isFishing == false)
        {
            isFishing = true;
            randomNumber = Random.Range(0, fishTypes.Count);
            selectedFish = fishTypes[randomNumber];
            StartCoroutine(WaitForBite());

            // do bullet hell thing here



        }
        
    }


    // Random time between fish biting hook
    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
    }
}
