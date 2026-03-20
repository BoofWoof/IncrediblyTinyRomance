using UnityEngine;

public class ShowOnDay : MonoBehaviour
{
    public bool Day0 = false;
    public bool Day1 = false;
    public bool Day2 = false;
    public bool Day3 = false;
    public bool Day4 = false;
    public bool Day5 = false;

    public void Awake()
    {
        if(DayInfo.CurrentDay == 0)
        {
            gameObject.SetActive(Day0);
        }
        else
        if (DayInfo.CurrentDay == 1)
        {
            gameObject.SetActive(Day1);
        }
        else
        if (DayInfo.CurrentDay == 2)
        {
            gameObject.SetActive(Day2);
        }
        else
        if (DayInfo.CurrentDay == 3)
        {
            gameObject.SetActive(Day3);
        }
        else
        if (DayInfo.CurrentDay == 4)
        {
            gameObject.SetActive(Day4);
        }
        else
        if (DayInfo.CurrentDay == 5)
        {
            gameObject.SetActive(Day5);
        }
    }
}
