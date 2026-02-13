using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class AttentionScript : MonoBehaviour
{
    public UnityEvent<bool> OnAttentionChange;

    private bool _AttentionActive = false;
    public bool AttentionActive {
        get {
            return _AttentionActive;
        }
        set {
            _AttentionActive = value;
            OnAttentionChange?.Invoke(value);
        } }

    private bool _FirstAttention = true;

    public void Start()
    {
        Lua.RegisterFunction("SetAttention", this, SymbolExtensions.GetMethodInfo(() => SetAttention()));
        Lua.RegisterFunction("ReleaseAttention", this, SymbolExtensions.GetMethodInfo(() => ReleaseAttention()));
        gameObject.SetActive(false);
    }

    public void SetAttention()
    {
        if (_FirstAttention) AnnouncementScript.StartAnnouncement("When <b>listen cat</b> appears on the left, feel free to keep doing puzzles and just easedrop.");
        _FirstAttention = false;

        AttentionActive = true;
        gameObject.SetActive(true);
    }
    public void ReleaseAttention()
    {
        AttentionActive = false;
        gameObject.SetActive(false);
    }
}
