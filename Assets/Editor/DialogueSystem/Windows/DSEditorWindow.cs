using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;

namespace DS.Windows
{
    using System.Diagnostics;
    using Utilities;
    public class DSEditorWindow : EditorWindow
    {
        private DSGraphView graphView;
        private readonly string defaultFileName = "DialogueFileName";

        private static TextField fileNameTextField;
        private Button saveButton;

        [MenuItem("Window/DS/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<DSEditorWindow>("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }
        #region Elements Addition

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DSElementUtility.CreateButton("Save", () => Save());

            Button loadButton = DSElementUtility.CreateButton("Load", () => Load());

            Button clearButton = DSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DSElementUtility.CreateButton("Reset", () => ResetGraph());
            Button miniMapButton = DSElementUtility.CreateButton("MiniMap", () => ToggleMiniMap());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets("Assets/EditorDefaultResources/DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddGraphView()
        {
            graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets(
                "Assets/EditorDefaultResources/DialogueSystem/DSVariables.uss"
            );
        }
        #endregion

        #region Toolbar Actions

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name.",
                    "Please ensure the file name you've typed in is valid.",
                    "Got it boss!"
                    );

                return;
            }
            DSIOUtility.Initialize(graphView, fileNameTextField.value);
            DSIOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty (filePath) )
            {
                return;
            }

            Clear();

            DSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DSIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();
            UpdateFileName(defaultFileName);
        }

        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();
        }

        #endregion

        #region Utility Methods
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}