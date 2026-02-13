using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityHealthScript : MonoBehaviour
{
    public TMP_Text HealthText;
    public Image HealthBar;
    public Image HealthBarContainer;

    public Sprite HighHealthSprite;
    public Sprite MediumHealthSprite;
    public Sprite LowHealthSprite;

    public void Start()
    {
        SetHealth(1);
        gameObject.SetActive(false);
    }
    public void SetHealth(float newHealth)
    {
        gameObject.SetActive(newHealth < 1);

        HealthText.text = (newHealth*100).ToString("F0") + "%";
        HealthBar.fillAmount = newHealth;

        if(newHealth > 0.9)
        {
            HealthBarContainer.sprite = HighHealthSprite;
        } else if (newHealth > 0.75)
        {
            HealthBarContainer.sprite = MediumHealthSprite;
        } else
        {
            HealthBarContainer.sprite= LowHealthSprite; 
        }
    }
}
