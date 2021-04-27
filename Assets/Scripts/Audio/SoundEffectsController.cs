using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsController : MonoBehaviour
    {
        //List of popping sounds tha tcan be played
        [SerializeField] AudioClip[] poppingSounds;

        AudioSource source;
        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        //Plays a random popping sound
        public void PlayPoppingEffect()
        {
            source.clip = poppingSounds[Random.Range(0, poppingSounds.Length)];
            source.Play();
        }
    }
}
