using UnityEngine;

public class StartGame : MonoBehaviour
{
    public FIshingSystem fishingSystem;
    public Animator animator;
    public UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        fishingSystem.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnGameStart()
    {
        fishingSystem.enabled = true;
        animator.SetBool("Start", true);
        uiManager.startCanvas.enabled = false;

    }
  
}
