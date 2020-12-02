using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI { 
    [RequireComponent(typeof(Bot))]
    public class BotPartAudioManager : MonoBehaviour
    {
        public enum BotAudioType
        {
            SFX,
            SPEECH,
            BGM
        }

        #region Components

        [SerializeField]
        protected AudioSource SFXAudioSourceComponent { get; set; }

        [SerializeField]
        protected AudioSource SpeechAudioSourceComponent { get; set; }

        [SerializeField]
        protected AudioSource BGMAudioSourceComponent { get; set; }

        #endregion Components

        
        #region Audio

        private AudioSource GetAudioSource(BotAudioType audioType)
        {
            switch (audioType)
            {
                case BotAudioType.SFX when SFXAudioSourceComponent != null:
                    return SFXAudioSourceComponent;
                case BotAudioType.SPEECH when SpeechAudioSourceComponent != null:
                    return SpeechAudioSourceComponent;
                case BotAudioType.BGM when BGMAudioSourceComponent != null:
                    return BGMAudioSourceComponent;
            }

            return null;
        }

        public void SetAudioClip(AudioClip clip, BotAudioType audioType)
        {
            GetAudioSource(audioType).clip = clip;
        }

        public bool IsAudioClipSet(AudioClip clip, BotAudioType audioType)
        {
            return GetAudioSource(audioType).clip == clip;

        }

        public bool IsAudioPlaying(BotAudioType audioType)
        {
            return GetAudioSource(audioType).isPlaying;
        }

        public bool PlayAudioClip(BotAudioType audioType, bool force = true)
        {
            if (force || !IsAudioPlaying(audioType))
            {
                GetAudioSource(audioType).Play();
                return true;
            }
            return false;
        }

        #region SFX

        protected void SetSFX(AudioClip clip)
        {
            SFXAudioSourceComponent.clip = clip;
        }

        protected bool IsSFXSet(AudioClip clip)
        {
            return SFXAudioSourceComponent.clip == clip;
        }

        public bool IsSFXPlaying
        {
            get
            {
                return SFXAudioSourceComponent.isPlaying;
            }
        }

        public void StopSFX()
        {
            SFXAudioSourceComponent.Stop();
        }

        public bool PlaySFX(bool force = true)
        {
            if (force || !IsSFXPlaying)
            {
                SFXAudioSourceComponent.Play();
                return true;
            }
            return false;
        }

        #endregion

        #region Speech

        protected void SetSpeech(AudioClip clip)
        {
            SpeechAudioSourceComponent.clip = clip;
        }

        protected bool IsSpeechSet(AudioClip clip)
        {
            return SpeechAudioSourceComponent.clip == clip;
        }

        public void StopSpeaking()
        {
            SpeechAudioSourceComponent.Stop();
        }

        public bool IsSpeaking
        {
            get
            {
                return SpeechAudioSourceComponent.isPlaying;
            }
        }

        public bool Speak(bool force = true)
        {
            if (force || !IsSpeaking)
            {
                SpeechAudioSourceComponent.Play();
                return true;
            }
            return false;
        }

        #endregion Speech
        #endregion Audio

    }
}