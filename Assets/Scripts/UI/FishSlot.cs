using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FishSlot : MonoBehaviour, IPointerClickHandler
{
    public string fishName;
    public SpriteRenderer fishSprite;
    public bool isFilled;
    public UnityEngine.UI.Image image;

    public GameObject selectedSahder;
    public bool thisFishSelected;

    private FishCollectionManager fishCollectionManager;
    


    private void Start()
    {
        fishCollectionManager = GameObject.Find("CollectionCanvas").GetComponent<FishCollectionManager>();
        
    }

    public void AddFish(string fishName, SpriteRenderer fishSprite)
    {
        this.fishName = fishName;
        this.fishSprite = fishSprite;
        isFilled = true;

        image.sprite = fishSprite.sprite;


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        fishCollectionManager.DeselectAllSlots();
        selectedSahder.SetActive(true);
        thisFishSelected = true;
    }
    public void OnRightClick()
    {
        
    }
}
