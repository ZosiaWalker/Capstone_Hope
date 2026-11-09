using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfPortBoolVariableView : VisualElement
    {
        public IfPortBoolVariableView(IfPortSO ifPort)
        {
            DropdownField operatorsDropdown = new DropdownField();
            if (operatorsDropdown != null)
            {
                operatorsDropdown.label = "Operator";
                operatorsDropdown.choices = Constants.SIMPLE_OPERATORS;
                operatorsDropdown.value = ifPort.operatorValue;
                operatorsDropdown.RegisterValueChangedCallback(evt =>
                {
                    ifPort.OnOperatorChanged(evt.newValue);
                });
                Add(operatorsDropdown);
            }

            DropdownField values = new DropdownField();
            if (values != null)
            {
                values.label = "Value";
                values.choices = Constants.BOOL_OPTIONS;
                values.value = ifPort.value;
                values.RegisterValueChangedCallback(evt =>
                {
                    ifPort.OnValueChanged(evt.newValue);
                });
                Add(values);
            }
        }
    }
}
