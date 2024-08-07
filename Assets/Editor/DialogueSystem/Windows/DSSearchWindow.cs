using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Windows
{
    using DS.Elements;
    using Enumerations;
    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DSGraphView dSGraphView)
        {
            graphView = dSGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.MultipleChoice
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                },
                new SearchTreeGroupEntry(new GUIContent("Save Group"), 1),
                new SearchTreeEntry(new GUIContent("Check Variable", indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.CheckVariable
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData)
            {
                case DSDialogueType.SingleChoice:
                    {
                        DSSingleChoiceNode singleChoiceNode = (DSSingleChoiceNode) graphView.CreateNode("DialogueNode", DSDialogueType.SingleChoice, localMousePosition);

                        graphView.AddElement(singleChoiceNode);
                        return true;
                    }
                case DSDialogueType.MultipleChoice:
                    {
                        DSMultipleChoiceNode multipleChoiceNode = (DSMultipleChoiceNode)graphView.CreateNode("DialogueNode", DSDialogueType.MultipleChoice, localMousePosition);

                        graphView.AddElement(multipleChoiceNode);
                        return true;
                    }
                case DSDialogueType.CheckVariable:
                    {
                        DSCheckNode checkVariableNode = (DSCheckNode)graphView.CreateNode("CheckNode", DSDialogueType.CheckVariable, localMousePosition);

                        graphView.AddElement(checkVariableNode);
                        return true;
                    }
                case Group _:
                    {
                        graphView.CreateGroup("DialogueGroup", localMousePosition);
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}