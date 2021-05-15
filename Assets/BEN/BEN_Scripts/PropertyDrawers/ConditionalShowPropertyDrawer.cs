using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
public class ConditionalShowPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalShowAttribute condHAtt = (ConditionalShowAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
 
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
         
        if (!condHAtt.hideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        } 
 
        GUI.enabled = wasEnabled; 
    } 
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalShowAttribute condHAtt = (ConditionalShowAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
 
        if (!condHAtt.hideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        { 
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
 
    private bool GetConditionalHideAttributeResult(ConditionalShowAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.conditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
 
        if (sourcePropertyValue != null) 
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else 
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.conditionalSourceField);
        }
 
        return enabled;
    }
}
