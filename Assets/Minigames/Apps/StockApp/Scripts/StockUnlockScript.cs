using UnityEngine;
using UnityEngine.UI;

public class StockUnlockScript : MonoBehaviour
{
    public Sprite LockSprite;

    public bool FlockLock = false;
    public bool AssscensssionLock = true;
    public bool FoundationLock = true;
    public bool RevolutionLock = true;

    public Image FlockButton;
    public Image AssscensssionButton;
    public Image FoundationButton;
    public Image RevolutionButton;

    [HideInInspector] public Sprite FlockHolder;
    [HideInInspector] public Sprite AssscensssionHolder;
    [HideInInspector] public Sprite FoundationHolder;
    [HideInInspector] public Sprite RevolutionHolder;

    public static StockUnlockScript instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        FlockHolder = FlockButton.sprite;
        AssscensssionHolder = AssscensssionButton.sprite;
        FoundationHolder = FoundationButton.sprite;
        RevolutionHolder = RevolutionButton.sprite;

        SetLockSprites();
    }

    // Update is called once per frame
    void SetLockSprites()
    {
        if (FlockLock)
        {
            FlockButton.sprite = LockSprite;
            FlockButton.GetComponent<Button>().interactable = false;
        }
        else { 
            FlockButton.sprite = FlockHolder;
            FlockButton.GetComponent<Button>().interactable = true;
        }

        if (AssscensssionLock)
        {
            AssscensssionButton.sprite = LockSprite;
            AssscensssionButton.GetComponent<Button>().interactable = false;
        }
        else { 
            AssscensssionButton.sprite = AssscensssionHolder;
            AssscensssionButton.GetComponent<Button>().interactable = true;
        }

        if (FoundationLock)
        {
            FoundationButton.sprite = LockSprite;
            FoundationButton.GetComponent<Button>().interactable = false;
        }
        else { 
            FoundationButton.sprite = FoundationHolder;
            FoundationButton.GetComponent<Button>().interactable = true;
        }

        if (RevolutionLock) { 
            RevolutionButton.sprite = LockSprite;
            RevolutionButton.GetComponent<Button>().interactable = false;
        }
        else { 
            RevolutionButton.sprite = RevolutionHolder;
            RevolutionButton.GetComponent<Button>().interactable = true;
        }
    }

    public void UnlockStock(StockNames stockNames)
    {
        if(stockNames == StockNames.Flock) FlockLock = false;
        if (stockNames == StockNames.Assscensssion) AssscensssionLock = false;
        if (stockNames == StockNames.Foundation) FoundationLock = false;
        if (stockNames == StockNames.Revolution) RevolutionLock = false;

        SetLockSprites();
    }
}
