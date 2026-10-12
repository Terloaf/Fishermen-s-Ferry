using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public GameObject fishCollectableMenu;
    private bool isMenuOpen;
    public FishSlot[] fishSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isMenuOpen)
        {
            Time.timeScale = 1;
            fishCollectableMenu.SetActive(false);
            isMenuOpen = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !isMenuOpen)
        {
            Time.timeScale = 0;
            fishCollectableMenu.SetActive(true);
            isMenuOpen = true;
        }
    }

    public void AddFish(string fishName, SpriteRenderer fishSprite)
    {
        Debug.Log("FishName = " + fishName + "Sprite = " + fishSprite);
        for (int i = 0; i < fishSlot.Length; i++)
        {
            if (fishSlot[i].isFilled == false)
            {
                fishSlot[i].AddFish(fishName, fishSprite);
                return;
            }
        }
    }
}
