using System.Threading.Tasks;
using UnityEngine;
using VInspector;

namespace UnityHelper
{
    public class AudioAction : MonoBehaviour
    {
        [SerializeField,Variants("VFX","Music")]
        private string   _soundType;
        
        [SerializeField]
        private KeySound _keySound;
        
        [SerializeField]
        private bool    _playOnEnable;
        
        [SerializeField]
        private bool   _playOnStart;
        
        [SerializeField]
        private float _delay;

        private void OnEnable()
        {
            if (!_playOnEnable) return;
            Play();
        }

        private void Start()
        {
            if(!_playOnStart) return;
            Play();
        }

        public async void Play()
        {
            await Task.Delay((int)(_delay * 1000));
            if(_soundType.Equals("VFX"))
                AudioManager.Instance.PlayOneShotSFX(_keySound);
            else
                AudioManager.Instance.PlayMusic(_keySound);
        }

        public async void PlayLoop()
        {
            await Task.Delay((int)(_delay * 1000));
            if(_soundType.Equals("VFX"))
                AudioManager.Instance.PlayOneShotSFX(_keySound);
            else
                AudioManager.Instance.PlayMusic(_keySound);
        }
        
    }
}