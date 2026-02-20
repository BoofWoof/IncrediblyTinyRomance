using PixelCrushers.DialogueSystem;
using UnityEngine;

public class LuaRegisterScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Lua.RegisterFunction("SetForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.SetForceEvent()));
        Lua.RegisterFunction("ReleaseForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.ReleaseForceEvent()));
    }
}
