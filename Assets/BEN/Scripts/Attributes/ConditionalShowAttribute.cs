using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalShowAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string conditionalSourceField;

    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool hideInInspector = false;

    public ConditionalShowAttribute(string conditionalSourceField)
    {
        this.conditionalSourceField = conditionalSourceField;
        hideInInspector = false;
    }

    public ConditionalShowAttribute(string conditionalSourceField, bool hideInInspector)
    {
        this.conditionalSourceField = conditionalSourceField;
        this.hideInInspector = hideInInspector;
    } 
}
