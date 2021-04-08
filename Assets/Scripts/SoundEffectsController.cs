using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsController : MonoBehaviour
    {
        [SerializeField] AudioClip[] poppingSounds;
        AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        public void PlayPoppingEffect()
        {
            source.clip = poppingSounds[Random.Range(0, poppingSounds.Length)];
            source.Play();
        }
    }
}
