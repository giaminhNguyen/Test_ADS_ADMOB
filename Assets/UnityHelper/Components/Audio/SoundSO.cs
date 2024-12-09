using System;
using UnityEngine;

namespace UnityHelper
{
    [CreateAssetMenu(fileName = "SoundSO", menuName = "DataSO/SoundSO")]
    public class SoundSO : ScriptableObject
    {
        public Sound[] sounds;
    }

    [Serializable]
    public struct Sound
    {
        public KeySound key;

        [Range(0.0f, 1.0f)]
        public float volume;

        public AudioClip soundClip;
    }
}