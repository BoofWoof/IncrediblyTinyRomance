using UnityEngine;
using UnityEngine.UI;

public class ReticleScript : MonoBehaviour
{
    public Sprite DefaultReticle;
    public Sprite InspectReticle;
    public Sprite QuestionReticle;

    public bool Inspecting = false;

    public static ReticleScript instance;

    public void SetDefault()
    {
        Inspecting = false;
        GetComponent<Image>().sprite = DefaultReticle;
    }

    public void SetInspector()
    {
        Inspecting = true;
        GetComponent<Image>().sprite = InspectReticle;
    }

    public void SetQuestion()
    {
        if (Inspecting) return;
        GetComponent<Image>().sprite = QuestionReticle;
    }

    public void OnEnable()
    {
        instance = this;
    }
}
