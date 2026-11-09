using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public partial class CustomFieldSO : ScriptableObject
    {
        [HideInInspector] public DictionarySerializable<string, string> stringFields = new DictionarySerializable<string, string>();
        [HideInInspector] public DictionarySerializable<string, int> intFields = new DictionarySerializable<string, int>();
        [HideInInspector] public DictionarySerializable<string, float> floatFields = new DictionarySerializable<string, float>();
        [HideInInspector] public DictionarySerializable<string, bool> boolFields = new DictionarySerializable<string, bool>();
        [HideInInspector] public DictionarySerializable<string, Vector2> vector2Fields = new DictionarySerializable<string, Vector2>();
        [HideInInspector] public DictionarySerializable<string, Vector3> vector3Fields = new DictionarySerializable<string, Vector3>();
        [HideInInspector] public DictionarySerializable<string, Sprite> spriteFields = new DictionarySerializable<string, Sprite>();
        [HideInInspector] public DictionarySerializable<string, GameObject> gameObjectFields = new DictionarySerializable<string, GameObject>();
        [HideInInspector] public DictionarySerializable<string, AudioClip> audioClipFields = new DictionarySerializable<string, AudioClip>();
        [HideInInspector] public DictionarySerializable<string, DialogueTreeSO> dialogueTreeFields = new DictionarySerializable<string, DialogueTreeSO>();

        private static T InitFields<T>(ref T field) where T : class, new() => field ??= new T();
        public DictionarySerializable<string, string> StringFields => InitFields(ref stringFields);
        public DictionarySerializable<string, int> IntFields => InitFields(ref intFields);
        public DictionarySerializable<string, float> FloatFields => InitFields(ref floatFields);
        public DictionarySerializable<string, bool> BoolFields => InitFields(ref boolFields);
        public DictionarySerializable<string, Vector2> Vector2Fields => InitFields(ref vector2Fields);
        public DictionarySerializable<string, Vector3> Vector3Fields => InitFields(ref vector3Fields);
        public DictionarySerializable<string, Sprite> SpriteFields => InitFields(ref spriteFields);
        public DictionarySerializable<string, GameObject> GameObjectFields => InitFields(ref gameObjectFields);
        public DictionarySerializable<string, AudioClip> AudioClipFields => InitFields(ref audioClipFields);
        public DictionarySerializable<string, DialogueTreeSO> DialogueTreeFields => InitFields(ref dialogueTreeFields);

        public static CustomFieldSO CreateInstance()
        {
            CustomFieldSO customFieldSO = ScriptableObject.CreateInstance<CustomFieldSO>();
            customFieldSO.name = customFieldSO.GetType().ToString();
            return customFieldSO;
        }

#if UNITY_EDITOR
        public void OnStringCustomFieldChanged(string fieldID, string value)
        {
            stringFields[fieldID] = value;
            Save();
        }

        public void OnIntCustomFieldChanged(string fieldID, int value)
        {
            intFields[fieldID] = value;
            Save();
        }

        public void OnFloatCustomFieldChanged(string fieldID, float value)
        {
            floatFields[fieldID] = value;
            Save();
        }

        public void OnBoolCustomFieldChanged(string fieldID, bool value)
        {
            boolFields[fieldID] = value;
            Save();
        }

        public void OnVector2CustomFieldChanged(string fieldID, Vector2 value)
        {
            vector2Fields[fieldID] = value;
            Save();
        }

        public void OnVector3CustomFieldChanged(string fieldID, Vector3 value)
        {
            vector3Fields[fieldID] = value;
            Save();
        }

        public void OnSpriteCustomFieldChanged(string fieldID, Sprite value)
        {
            spriteFields[fieldID] = value;
            Save();
        }

        public void OnGameObjectCustomFieldChanged(string fieldID, GameObject value)
        {
            gameObjectFields[fieldID] = value;
            Save();
        }

        public void OnAudioClipCustomFieldChanged(string fieldID, AudioClip value)
        {
            audioClipFields[fieldID] = value;
            Save();
        }

        public void OnDialogueTreeCustomFieldChanged(string fieldID, DialogueTreeSO value)
        {
            dialogueTreeFields[fieldID] = value;
            Save();
        }

        public void Save()
        {
            if (!EditorUtility.IsDirty(this))
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void CopyFrom(CustomFieldSO other)
        {
            stringFields = other.stringFields.Clone();
            intFields = other.intFields.Clone();
            floatFields = other.floatFields.Clone();
            boolFields = other.boolFields.Clone();
            vector2Fields = other.vector2Fields.Clone();
            vector3Fields = other.vector3Fields.Clone();
            spriteFields = other.spriteFields.Clone();
            gameObjectFields = other.gameObjectFields.Clone();
            audioClipFields = other.audioClipFields.Clone();
            dialogueTreeFields = other.dialogueTreeFields.Clone();

            const string methodName = "CopyEnumsFrom";

            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(this, new object[] { other });
            }
            else
            {
                Debug.LogWarning($"Some data couldn't be copied properly, because the {methodName} function has not been generated yet. \nPlease regenerate code via Tools/Dialogue System/Regenerate Code to generate this function.");
            }
        }
#endif

        public T GetCustomFieldValue<T>(FieldSO fieldSO)
        {
            object value = null;

            if (fieldSO == null)
            {
                return (T)value;
            }

            if (typeof(T) == typeof(string))
            {
                if (stringFields.ContainsKey(fieldSO.FieldID))
                    value = stringFields[fieldSO.FieldID];
                else
                    value = "";
            }
            else if (typeof(T) == typeof(int))
            {
                if (intFields.ContainsKey(fieldSO.FieldID))
                    value = intFields[fieldSO.FieldID];
                else
                    value = 0;
            }
            else if (typeof(T) == typeof(float))
            {
                if (floatFields.ContainsKey(fieldSO.FieldID))
                    value = floatFields[fieldSO.FieldID];
                else
                    value = 0f;
            }
            else if (typeof(T) == typeof(bool))
            {
                if (boolFields.ContainsKey(fieldSO.FieldID))
                    value = boolFields[fieldSO.FieldID];
                else
                    value = false;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                if (vector2Fields.ContainsKey(fieldSO.FieldID))
                    value = vector2Fields[fieldSO.FieldID];
                else
                    value = Vector2.zero;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                if (vector3Fields.ContainsKey(fieldSO.FieldID))
                    value = vector3Fields[fieldSO.FieldID];
                else
                    value = Vector3.zero;
            }
            else if (typeof(T) == typeof(Sprite) && spriteFields.ContainsKey(fieldSO.FieldID))
            {
                value = spriteFields[fieldSO.FieldID];
            }
            else if (typeof(T) == typeof(GameObject) && gameObjectFields.ContainsKey(fieldSO.FieldID))
            {
                value = gameObjectFields[fieldSO.FieldID];
            }
            else if (typeof(T) == typeof(AudioClip) && audioClipFields.ContainsKey(fieldSO.FieldID))
            {
                value = audioClipFields[fieldSO.FieldID];
            }
            else if (typeof(T) == typeof(DialogueTreeSO) && dialogueTreeFields.ContainsKey(fieldSO.FieldID))
            {
                value = dialogueTreeFields[fieldSO.FieldID];
            }
            else
            {
                string methodName = "GetEnumValue";

                MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (method != null)
                {
                    value = method.Invoke(this, new object[] { fieldSO });
                    //if (value == null)
                    //{
                    //    throw new NotSupportedException($"Type {typeof(T)} is not supported.");
                    //}
                }
                else
                {
                    Debug.LogError($"Method '{methodName}' not found on type {GetType().Name}");
                }
            }

            return (T)value;
        }
    }
}
