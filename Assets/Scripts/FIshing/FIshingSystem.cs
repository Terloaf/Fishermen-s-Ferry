using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FIshingSystem : MonoBehaviour
{
    private bool isFishing;
    private int randomNumber;
    public AnimationClip castingLine;
    public Animator animator;

    // list of each fish that can potentially be caught
    public List<GameObject> fishTypes = new List<GameObject>();

    // the currently selected fish out of the list
    public GameObject selectedFish;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isFishing == false) // && bullet hell thing isnt active
            {
                // ignore this for now
                if(animator.GetBool("isCasting") == false)
                {
                    animator.SetBool("isCasting", true);
                }
                else if(animator.GetBool("isCasting") == true)
                {
                    animator.SetBool("isCasting", false);
                }
                

                isFishing = true;
                randomNumber = Random.Range(0, fishTypes.Count);
                selectedFish = fishTypes[randomNumber];
                StartCoroutine(WaitForBite());
                isFishing = false;

                // do bullet hell thing here
            }
        }

    }


    public void OnLeftClick()
    {

        
    }


    // Random time between fish biting hook
    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
    }
}
