using UnityEngine;

public class FishSlot : MonoBehaviour
{
    public string fishName;
    public SpriteRenderer fishSprite;
    public bool isFilled;
    public UnityEngine.UI.Image image;

    


    public void AddFish(string fishName, SpriteRenderer fishSprite)
    {
        this.fishName = fishName;
        this.fishSprite = fishSprite;
        isFilled = true;

        image.sprite = fishSprite.sprite;

    }

}
