using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace DS.Characters
{
    using Utilities;
    [CustomEditor(typeof(CharacterInfo))]
    public class CharacterInfoEditor : Editor
    {
        //Default Values
        private SerializedProperty characterName;
        private SerializedProperty defaultCharacterSprite;
        private SerializedProperty defaultCharacterNoise;

        //Pseudo Dictionaries
        public List<string> spriteUuid;
        public List<string> emotionSpriteNames;
        public List<Sprite> emotionSprites;

        public List<string> noiseUuid;
        public List<string> emotionNoiseNames;
        public List<AudioClip> emotionNoises;

        string newKeySprite = "";
        Sprite newValueSprite = null;

        string newKeyNoise = "";
        AudioClip newValueNoise = null;

        private void OnEnable()
        {
            //Default Values
            characterName = serializedObject.FindProperty("characterName");
            defaultCharacterSprite = serializedObject.FindProperty("defaultCharacterSprite");
            defaultCharacterNoise = serializedObject.FindProperty("defaultCharacterNoise");

            //Pseudo Dictionaries
            CharacterInfo characterInfo = (CharacterInfo)target;

            emotionSpriteNames = characterInfo.emotionSpriteNames;
            emotionSprites = characterInfo.emotionSprites;
            spriteUuid = characterInfo.spriteUuid;
            if (emotionSprites == null)
            {
                spriteUuid = new List<string>();
                emotionSpriteNames = new List<string>();
                emotionSprites = new List<Sprite> ();
            }
            emotionNoiseNames = characterInfo.emotionNoiseNames;
            emotionNoises = characterInfo.emotionNoises;
            noiseUuid = characterInfo.noiseUuid;
            if (emotionNoises == null)
            {
                noiseUuid = new List<string>();
                emotionNoiseNames = new List<string>();
                emotionNoises = new List<AudioClip>();
            }
        }

        public override void OnInspectorGUI()
        {
            DSInspectorUtility.DrawHeader("Default Values");
            characterName.DrawPropertyField();
            defaultCharacterSprite.DrawPropertyField();
            defaultCharacterNoise.DrawPropertyField();

            DrawSpriteEmotionArea();

            DrawNoiseEmotionArea();
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        #region Draw
        private void DrawSpriteEmotionArea()
        {

            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHeader("Emotion Sprites");

            List<string> keyList = emotionSpriteNames;
            List<Sprite> valueList = emotionSprites;

            DrawSpriteDictionary(keyList, valueList);
        }
        private void DrawNoiseEmotionArea()
        {

            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHeader("Emotion Noises");

            List<string> keyList = emotionNoiseNames;
            List<AudioClip> valueList = emotionNoises;

            DrawAudioDictionary(keyList, valueList);
        }

        private void DrawSpriteDictionary(List<string> keyList, List<Sprite> valueList)
        {
            List<string> uuidToRemove = new List<string>();
            List<string> keysToRemove = new List<string>();
            List<Sprite> valuesToRemove = new List<Sprite>();
            for (int idx = 0; idx < keyList.Count; idx++)
            {
                string current_key = keyList[idx];
                Sprite current_value = valueList[idx];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                {
                    keysToRemove.Add(current_key);
                    valuesToRemove.Add(current_value);
                    uuidToRemove.Add(spriteUuid[idx]);
                    continue;
                }
                string new_key = EditorGUILayout.TextField(current_key);
                Sprite new_value = (Sprite)EditorGUILayout.ObjectField(current_value, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();

                // If key has changed, update the dictionary
                if (new_key != current_key)
                {
                    if (!keyList.Contains(new_key))
                    {
                        keyList.Remove(current_key);
                        valueList.Remove(current_value);
                        keyList.Insert(idx, new_key);
                        valueList.Insert(idx, new_value);
                    }
                }
                // If value has changed, update the dictionary
                else if (new_value != current_value)
                {
                    valueList[idx] = new_value;
                }
            }
            foreach (string key in keysToRemove)
            {
                keyList.Remove(key);
            }
            foreach (Sprite value in valuesToRemove)
            {
                valueList.Remove(value);
            }
            foreach (string uuid in uuidToRemove)
            {
                spriteUuid.Remove(uuid);
            }

            // Add new key-value pair
            DSInspectorUtility.DrawSpace();
            DSInspectorUtility.DrawHeader("Add New Sprites");
            EditorGUILayout.BeginHorizontal();
            newKeySprite = EditorGUILayout.TextField("New Key", newKeySprite);
            newValueSprite = (Sprite)EditorGUILayout.ObjectField(newValueSprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add New"))
            {
                // Add new key-value pair to the dictionary
                if (!string.IsNullOrEmpty(newKeySprite))
                {
                    if (!keyList.Contains(newKeySprite))
                    {
                        spriteUuid.Add(Guid.NewGuid().ToString());
                        keyList.Add(newKeySprite);
                        valueList.Add(newValueSprite);
                    }
                }
            }
        }
        private void DrawAudioDictionary(List<string> keyList, List<AudioClip> valueList)
        {
            List<string> uuidToRemove = new List<string>();
            List<string> keysToRemove = new List<string>();
            List<AudioClip> valuesToRemove = new List<AudioClip>();
            for (int idx = 0; idx < keyList.Count; idx++)
            {
                string current_key = keyList[idx];
                AudioClip current_value = valueList[idx];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                {
                    keysToRemove.Add(current_key);
                    valuesToRemove.Add(current_value);
                    uuidToRemove.Add(noiseUuid[idx]);
                    continue;
                }
                string new_key = EditorGUILayout.TextField(current_key);
                AudioClip new_value = (AudioClip)EditorGUILayout.ObjectField(current_value, typeof(AudioClip), false);
                EditorGUILayout.EndHorizontal();

                // If key has changed, update the dictionary
                if (new_key != current_key)
                {
                    if (!keyList.Contains(new_key))
                    {
                        keyList.Remove(current_key);
                        valueList.Remove(current_value);
                        keyList.Insert(idx, new_key);
                        valueList.Insert(idx, new_value);
                    }
                }
                // If value has changed, update the dictionary
                else if (new_value != current_value)
                {
                    valueList[idx] = new_value;
                }
            }
            foreach (string key in keysToRemove)
            {
                keyList.Remove(key);
            }
            foreach (AudioClip value in valuesToRemove)
            {
                valueList.Remove(value);
            }
            foreach (string uuid in uuidToRemove)
            {
                noiseUuid.Remove(uuid);
            }

            // Add new key-value pair
            DSInspectorUtility.DrawSpace();
            DSInspectorUtility.DrawHeader("Add New AudioClips");
            EditorGUILayout.BeginHorizontal();
            newKeyNoise = EditorGUILayout.TextField("New Key", newKeyNoise);
            newValueNoise = (AudioClip)EditorGUILayout.ObjectField(newValueNoise, typeof(AudioClip), false);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add New"))
            {
                // Add new key-value pair to the dictionary
                if (!string.IsNullOrEmpty(newKeyNoise))
                {
                    if (!keyList.Contains(newKeyNoise))
                    {
                        noiseUuid.Add(Guid.NewGuid().ToString());
                        keyList.Add(newKeyNoise);
                        valueList.Add(newValueNoise);
                    }
                }
            }
        }


        #endregion

    }

}
