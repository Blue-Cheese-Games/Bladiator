using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Sound
{
    public class TemporaryAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource m_source;

        private void Update()
        {
            if (!m_source.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}