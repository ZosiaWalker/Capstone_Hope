using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
    public partial class CustomFieldView : VisualElement
    {
        public CustomFieldSO customFieldSO;

        public CustomFieldView(HashSet<FieldSO> fields, CustomFieldSO customFieldSO)
        {
            this.customFieldSO = customFieldSO;
            LoadUIElements(fields);
        }

        void LoadUIElements(HashSet<FieldSO> fields)
        {
            List<string> fieldNames = new List<string>();
            foreach (FieldSO fieldSO in fields)
            {
                string fieldName = fieldSO.label;
                if (fieldSO == null || fieldNames.Contains(fieldName))
                {
                    continue;
                }
                if (fieldSO.fieldType == CustomFieldType.String)
                {
                    TextField textField = new TextField(fieldSO.label);
                    if (customFieldSO.StringFields.ContainsKey(fieldSO.FieldID))
                    {
                        textField.value = customFieldSO.StringFields[fieldSO.FieldID];
                    }

                    textField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnStringCustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(textField);
                }
                else if (fieldSO.fieldType == CustomFieldType.Int)
                {
                    IntegerField integerField = new IntegerField(fieldSO.label);
                    if (customFieldSO.IntFields.ContainsKey(fieldSO.FieldID))
                    {
                        integerField.value = customFieldSO.IntFields[fieldSO.FieldID];
                    }

                    integerField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnIntCustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(integerField);
                }
                else if (fieldSO.fieldType == CustomFieldType.Float)
                {
                    FloatField floatField = new FloatField(fieldSO.label);
                    if (customFieldSO.FloatFields.ContainsKey(fieldSO.FieldID))
                    {
                        floatField.value = customFieldSO.FloatFields[fieldSO.FieldID];
                    }

                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnFloatCustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(floatField);
                }
                else if (fieldSO.fieldType == CustomFieldType.Bool)
                {
                    Toggle toggleField = new Toggle(fieldSO.label);
                    if (customFieldSO.BoolFields.ContainsKey(fieldSO.FieldID))
                    {
                        toggleField.value = customFieldSO.BoolFields[fieldSO.FieldID];
                    }

                    toggleField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnBoolCustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(toggleField);
                }
                else if (fieldSO.fieldType == CustomFieldType.Enum)
                {
                    object value = null;
                    string methodName = "GetEnumField";

                    MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    if (method != null)
                    {
                        value = method.Invoke(this, new object[] { fieldSO });
                        if (value != null)
                        {
                            Add(value as EnumField);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Method '{methodName}' not found on type {GetType().Name}");
                    }

                    //EnumField enumField = GetEnumField(fieldSO);
                    //if (enumField != null)
                    //{
                    //    Add(enumField);
                    //}
                }
                else if (fieldSO.fieldType == CustomFieldType.Vector2)
                {
                    Vector2Field vector2Field = new Vector2Field(fieldSO.label);
                    if (customFieldSO.Vector2Fields.ContainsKey(fieldSO.FieldID))
                    {
                        vector2Field.value = customFieldSO.Vector2Fields[fieldSO.FieldID];
                    }

                    vector2Field.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnVector2CustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(vector2Field);
                }
                else if (fieldSO.fieldType == CustomFieldType.Vector3)
                {
                    Vector3Field vector3Field = new Vector3Field(fieldSO.label);
                    if (customFieldSO.Vector3Fields.ContainsKey(fieldSO.FieldID))
                    {
                        vector3Field.value = customFieldSO.Vector3Fields[fieldSO.FieldID];
                    }

                    vector3Field.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnVector3CustomFieldChanged(fieldSO.FieldID, evt.newValue);
                    });

                    Add(vector3Field);
                }
                else if (fieldSO.fieldType == CustomFieldType.Sprite)
                {
                    ObjectField spriteField = new ObjectField(fieldSO.label)
                    {
                        objectType = typeof(Sprite),
                        allowSceneObjects = false
                    };
                    if (customFieldSO.SpriteFields.ContainsKey(fieldSO.FieldID))
                    {
                        spriteField.value = customFieldSO.SpriteFields[fieldSO.FieldID];
                    }

                    spriteField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnSpriteCustomFieldChanged(fieldSO.FieldID, evt.newValue as Sprite);
                    });

                    Add(spriteField);
                }
                else if (fieldSO.fieldType == CustomFieldType.GameObject)
                {
                    ObjectField gameObjectField = new ObjectField(fieldSO.label)
                    {
                        objectType = typeof(GameObject),
                        allowSceneObjects = true
                    };
                    if (customFieldSO.GameObjectFields.ContainsKey(fieldSO.FieldID))
                    {
                        gameObjectField.value = customFieldSO.GameObjectFields[fieldSO.FieldID];
                    }

                    gameObjectField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnGameObjectCustomFieldChanged(fieldSO.FieldID, evt.newValue as GameObject);
                    });

                    Add(gameObjectField);
                }
                else if (fieldSO.fieldType == CustomFieldType.AudioClip)
                {
                    ObjectField audioClipField = new ObjectField(fieldSO.label)
                    {
                        objectType = typeof(UnityEngine.AudioClip),
                        allowSceneObjects = false
                    };
                    if (customFieldSO.AudioClipFields.ContainsKey(fieldSO.FieldID))
                    {
                        audioClipField.value = customFieldSO.AudioClipFields[fieldSO.FieldID];
                    }

                    audioClipField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnAudioClipCustomFieldChanged(fieldSO.FieldID, evt.newValue as AudioClip);
                    });

                    Add(audioClipField);
                }
                else if (fieldSO.fieldType == CustomFieldType.DialogueTree)
                {
                    ObjectField dialogueTreeField = new ObjectField(fieldSO.label)
                    {
                        objectType = typeof(DialogueTreeSO),
                        allowSceneObjects = false
                    };
                    if (customFieldSO.DialogueTreeFields.ContainsKey(fieldSO.FieldID))
                    {
                        dialogueTreeField.value = customFieldSO.DialogueTreeFields[fieldSO.FieldID];
                    }

                    dialogueTreeField.RegisterValueChangedCallback(evt =>
                    {
                        customFieldSO.OnDialogueTreeCustomFieldChanged(fieldSO.FieldID, evt.newValue as DialogueTreeSO);
                    });

                    Add(dialogueTreeField);
                }
                fieldNames.Add(fieldName);
            }
        }
    }
}
