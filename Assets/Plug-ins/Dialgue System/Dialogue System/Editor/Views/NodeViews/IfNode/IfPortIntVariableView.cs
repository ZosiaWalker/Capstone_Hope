using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfPortIntVariableView : VisualElement
    {
        public IfPortIntVariableView(IfPortSO ifPort)
        {
            DropdownField operatorsDropdown = new DropdownField();
            if (operatorsDropdown != null)
            {
                operatorsDropdown.label = "Operator";
                operatorsDropdown.choices = Constants.ALL_OPERATORS;
                operatorsDropdown.value = ifPort.operatorValue;
                operatorsDropdown.RegisterValueChangedCallback(evt =>
                {
                    ifPort.OnOperatorChanged(evt.newValue);
                });
                Add(operatorsDropdown);
            }

            IntegerField values = new IntegerField();
            if (values != null)
            {
                values.label = "Value";
                int temp = 0;
                if (int.TryParse(ifPort.value, out temp))
                {
                    values.value = temp;
                }
                values.RegisterValueChangedCallback(evt =>
                {
                    ifPort.OnValueChanged(evt.newValue.ToString());
                });
                Add(values);
            }
        }
    }
}
