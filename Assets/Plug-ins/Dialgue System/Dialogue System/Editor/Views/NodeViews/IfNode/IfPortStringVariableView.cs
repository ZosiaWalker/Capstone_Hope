using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfPortStringVariableView : VisualElement
    {
        public IfPortStringVariableView(IfPortSO ifPort)
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

            TextField values = new TextField();
            if (values != null)
            {
                values.label = "Value";
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
