using PixelCrushers.DialogueSystem;
using UnityEngine;

public class LuaRegisterScript : MonoBehaviour
{
    void Start()
    {
        Lua.RegisterFunction("SetForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.SetForceEvent()));
        Lua.RegisterFunction("ReleaseForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.ReleaseForceEvent()));
    }
}
