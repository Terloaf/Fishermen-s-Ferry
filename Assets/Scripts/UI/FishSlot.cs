using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

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
