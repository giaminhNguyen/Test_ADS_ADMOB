using System;
using UnityEngine;

namespace UnityHelper
{
    public class AudioManager : MonoBehaviour
    {
        #region Properties

        private static AudioManager ins;
        public static  AudioManager Instance => ins;
    
        [Header("Reference")]
        [SerializeField]
        private AudioSource _audioMusic;
        [SerializeField]
        private AudioSource _audioSFX;
        [SerializeField]
        private SoundSO _soundSO;

        public  bool ActiveSFX   { get; private set; }
        public  bool ActiveMUSIC { get; private set; }
        
        public bool MusicState => _musicState;
        
        private int  _soundLength;
        private bool _musicState;
        private bool _sfxState;
    
        #endregion
        
        private void Awake()
        {
            if (!ins)
            {
                ins = this;
            }

            _soundLength = _soundSO.sounds.Length;
            StartMusicAwake();
        }
        
        private void StartMusicAwake()
        {
            if(_soundLength == 0) return;
            var sound = Array.Find(_soundSO.sounds, s => s.key == KeySound.StartAwake);

            if (sound.Equals(default))
            {
                return;
            }

            _musicState        = true;
            _audioMusic.clip   = sound.soundClip;
            _audioMusic.volume = sound.volume;
            PlayMusic();
        }
        
        public void PlayOneShotSFX(KeySound key)
        {
            if (!ActiveSFX || _soundLength == 0)
            {
                return;
            }

            var sound = Array.Find(_soundSO.sounds, s => s.key == key);
            
            if (sound.Equals(default))
            {
                return;
            }
            
            _audioSFX.PlayOneShot(sound.soundClip, sound.volume);
        }
        public void PlaySFX(KeySound key, bool loop = false)
        {
            if (!ActiveSFX || _soundLength == 0)
            {
                return;
            }
            var sound = Array.Find(_soundSO.sounds, s => s.key == key);
            
            if (sound.Equals(default))
            {
                return;
            }
            _audioSFX.volume = sound.volume;
            _audioSFX.clip   = sound.soundClip;
            _audioSFX.loop   = loop;
            PlaySFX();
        }
        public void PlaySFX()
        {
            if(!ActiveSFX) return;
            _audioSFX.Play();
        }
        public void StopSFX()
        {
            if(!ActiveSFX) return;
            _audioSFX.Stop();
        }
        
        public void PlayMusic(KeySound key,bool loop = false)
        {
            if (!ActiveMUSIC || _soundLength == 0)
            {
                return;
            }

            var sound = Array.Find(_soundSO.sounds, s => s.key == key);
            
            if (sound.Equals(default))
            {
                _musicState = true;
                return;
            }
            
            _audioMusic.clip   = sound.soundClip;
            _audioMusic.volume = sound.volume;
            _audioMusic.loop   = loop;
            PlayMusic();
        }
        public void PlayMusic()
        {
            _musicState = true;
            if(!ActiveMUSIC) return;
            _audioMusic.Play();
        }
        public void PauseMusic()
        {
            _musicState = false;
            if(!ActiveMUSIC) return;
            _audioMusic.Pause();
        }
        public void ActiveSfx(bool active)
        {
            ActiveSFX        = active;

            if (!active)
            {
                _audioSFX.Stop();
            }
        }
        public void ActiveMusic(bool active)
        {
            ActiveMUSIC        = active;

            if (active)
            {
                if(!_musicState) return;
                _audioMusic.Play();
            }else
            {
                _audioMusic.Pause();
            }
        }

    }
}