using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace DebugTools.DeveloperConsole.Commands
{
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

        private static DeveloperConsoleBehaviour instance;

        private DeveloperConsole developerConsole;

        private DeveloperConsole DeveleoperConsole
        {
            get
            {
                if (developerConsole != null) { return developerConsole; }
                return developerConsole = new DeveloperConsole(prefix, commands);
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (!context.action.triggered) { return; }

            if (uiCanvas.activeSelf)
            {
                uiCanvas.SetActive(false);
            } else
            {
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }
        public void SubmitCommand(InputAction.CallbackContext context)
        {
            if (!context.action.triggered) { return; }

            if (uiCanvas.activeSelf)
            {
                ProcessCommand(inputField.text);
            }
        }

        public void ProcessCommand(string inputValue)
        {
            DeveleoperConsole.ProcessCommand(inputValue);

            inputField.text = string.Empty;

            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }
    }
}
