using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField]
    public GameObject obj;
    public ShopList shopList;

    [System.NonSerialized]
    static public int CurrentLevel;
    static public int CurrentPage;
    private Vector3 Position;
    private SpriteRenderer spriteR;
    
    private void Awake()
    {
        if (!obj.GetComponent<SpriteRenderer>())
        {
            obj.AddComponent<SpriteRenderer>();
        }

        obj.transform.localScale = new Vector3(2, 2, 0);
        Position = new Vector3(-6, -1, 0);
        obj.transform.position = Position;
        
        spriteR = obj.GetComponent<SpriteRenderer>();
        spriteR.sortingLayerName = "Items";

        CurrentLevel = CurrentPage = 0;
        spriteR.sprite = shopList.Level[CurrentLevel].Sprite_List[CurrentPage];
    }
    
    public void Update()
    {
        // Update CurrentPage
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (CurrentPage != 0)
                CurrentPage--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (CurrentPage != shopList.Level[CurrentLevel].Sprite_List.Count-1)
                CurrentPage++;
        }

        // Update CurrentLevel
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (CurrentLevel != 0)
            {
                CurrentLevel--;
                CurrentPage = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (CurrentLevel != shopList.Level.Count-1)
            {
                CurrentLevel++;
                CurrentPage = 0;
            }
        }

        spriteR.sprite = shopList.Level[CurrentLevel].Sprite_List[CurrentPage];
        SetSpritePosition();
    }

    public void SetSpritePosition()
    {
        Position = new Vector3(-6, -1, 0);
        if (CurrentLevel == 1)
        {
            if (CurrentPage == 0)
                Position = new Vector3(-6,-0.2f,0);

            else
                Position = new Vector3(-6, -1, 0);
        }

        if (CurrentLevel == 2)
        {
            if(CurrentPage <= 1)
                Position = new Vector3(-6, 1.5f, 0);

            else if (CurrentPage == 2)
                Position = new Vector3(-6, 1.85f, 0);

            else
                Position = new Vector3(-6, 1.6f, 0);
        }

        if (CurrentLevel == 3)
        {
            if (CurrentPage <= 2)
                Position = new Vector3(-6, 1.55f, 0);

            else
                Position = new Vector3(-6, 1.15f, 0);
        }

        if (CurrentLevel == 4)
        {
            if (CurrentPage == 0)
                Position = new Vector3(-6, 0.2f, 0);

            else if (CurrentPage <= 2)
                Position = new Vector3(-6, 1.5f, 0);

            else
                Position = new Vector3(-6, -0.1f, 0);
        }

        obj.transform.position = Position;
    }
}
