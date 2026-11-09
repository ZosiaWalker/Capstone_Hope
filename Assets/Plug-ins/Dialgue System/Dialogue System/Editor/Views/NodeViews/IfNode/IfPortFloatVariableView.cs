using UnityEngine.UIElements;

namespace DialogueSystem
{
    public class IfPortFloatVariableView : VisualElement
    {
        public IfPortFloatVariableView(IfPortSO ifPort)
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

            FloatField values = new FloatField();
            if (values != null)
            {
                values.label = "Value";
                float temp = 0f;
                if (float.TryParse(ifPort.value, out temp))
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
