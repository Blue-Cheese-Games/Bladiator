using UnityEngine;

namespace Bladiator.UI
{
    public class VolumeTest : MonoBehaviour
    {
        [SerializeField] private AudioSource m_SoundSliderTestSound;
        [SerializeField] private AudioClip m_SoundSliderVolume;

        public static float Volume;
        
        public void BTN_TestAudio()
        {
            m_SoundSliderTestSound.volume = Volume;
            m_SoundSliderTestSound.clip = m_SoundSliderVolume;
            m_SoundSliderTestSound.Play();
        }
    }
}
