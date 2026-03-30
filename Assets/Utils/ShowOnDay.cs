using UnityEngine;

public class ShowOnDay : MonoBehaviour
{
    public bool Day0 = false;
    public bool Day1 = false;
    public bool Day2 = false;
    public bool Day3 = false;
    public bool Day4 = false;
    public bool Day5 = false;

    public bool DeleteInsteadOfDisable = false;

    public void Awake()
    {
        if(DayInfo.CurrentDay == 0)
        {
            if(DeleteInsteadOfDisable && !Day0) Destroy(gameObject);
            else gameObject.SetActive(Day0);
        }
        else
        if (DayInfo.CurrentDay == 1)
        {
            if (DeleteInsteadOfDisable && !Day1) Destroy(gameObject);
            else gameObject.SetActive(Day1);
        }
        else
        if (DayInfo.CurrentDay == 2)
        {
            if (DeleteInsteadOfDisable && !Day2) Destroy(gameObject);
            else gameObject.SetActive(Day2);
        }
        else
        if (DayInfo.CurrentDay == 3)
        {
            if (DeleteInsteadOfDisable && !Day3) Destroy(gameObject);
            else gameObject.SetActive(Day3);
        }
        else
        if (DayInfo.CurrentDay == 4)
        {
            if (DeleteInsteadOfDisable && !Day4) Destroy(gameObject);
            else gameObject.SetActive(Day4);
        }
        else
        if (DayInfo.CurrentDay == 5)
        {
            if (DeleteInsteadOfDisable && !Day5) Destroy(gameObject);
            else gameObject.SetActive(Day5);
        }
    }
}
