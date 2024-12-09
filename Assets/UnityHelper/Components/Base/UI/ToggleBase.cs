using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace UnityHelper
{
    public abstract class ToggleBase : MonoBehaviour,IOnChangeValue
    {
        [Tab("Base")]
        [SerializeField]
        protected Toggle _toggle;

        [EndIf]
        [EndTab]

        private void OnValidate()
        {
            if (!_toggle)
            {
                _toggle = GetComponent<Toggle>();
            }
        }

        protected virtual void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnChangeValue);
        }

        protected virtual void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnChangeValue);
        }

        public abstract void OnChangeValue<T>(T value);
        
        public virtual void ChangeOnlyValueState(bool state)
        {
            _toggle.isOn = state;
        }
    }
}