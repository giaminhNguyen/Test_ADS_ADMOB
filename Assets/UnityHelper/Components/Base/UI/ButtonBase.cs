using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace UnityHelper
{
    public abstract class ButtonBase : MonoBehaviour,IOnClick,IOnChangeValue
    {
        [Tab("Base")]
        [SerializeField]
        protected Button _button;
    
        [SerializeField, Variants("One Way", "Two Way")]
        protected string _buttonTypeWay;
    
        protected bool _currentState;
        [EndTab]
        //
    
        private void OnValidate()
        {
            if (!_button)
            {
                _button = GetComponent<Button>();
            }
        }
    
        protected virtual void OnEnable()
        {
            if (_buttonTypeWay.Equals("One Way"))
            {
                _button.onClick.AddListener(OnClick);
            }
            else
            {
                _button.onClick.AddListener(OnAddActionChangeValue);
            }
        }
    
        private void OnDisable()
        {
            if (_buttonTypeWay.Equals("One Way"))
            {
                _button.onClick.RemoveListener(OnClick);
            }
            else
            {
                _button.onClick.RemoveListener(OnAddActionChangeValue);
            }
        }
    
        protected virtual void OnAddActionChangeValue()
        {
            if(_buttonTypeWay.Equals("One Way")) return;
            _currentState = !_currentState;
            OnChangeValue(_currentState);
        }
        
        public void ChangeOnlyValueState(bool state)
        {
            if(_buttonTypeWay.Equals("One Way")) return;
            _currentState = state;
        }
            
        public void ChangeState(bool state)
        {
            ChangeOnlyValueState(state);
            OnChangeValue(state);
        }

    
        public abstract void OnClick();
        public abstract void OnChangeValue<T>(T value);
    }
}

