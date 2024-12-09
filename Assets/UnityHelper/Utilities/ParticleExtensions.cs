using UnityEngine;

namespace UnityHelper
{
    public static class ParticleExtensions
    {
        public static void Restart(this ParticleSystem particleSystem)
        {
            particleSystem.Stop();
            particleSystem.Clear();
            particleSystem.Play();
        }
    }
}