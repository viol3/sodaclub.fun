using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.Audio
{
    public class AudioPool : LocalSingleton<AudioPool>
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private int _poolCount = 5;

        private AudioSource[] _audioSources;
        private Coroutine _manualLoopCo;
        protected override void Awake()
        {
            base.Awake();
            _audioSources = new AudioSource[_poolCount];
            for (int i = 0; i < _poolCount; i++)
            {
                _audioSources[i] = gameObject.AddComponent<AudioSource>();
                _audioSources[i].playOnAwake = false;
            }
        }

        public bool IsAnyClipPlayingContains(string name)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if(_audioSources[i].isPlaying && _audioSources[i].clip.name.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public void PlayRandomCardFlip()
        {
            int randomIndex = Random.Range(1, 4);
            PlayClipByName("card-flip-" + randomIndex, false, 0.5f);
        }

        public void PlayCardSpread()
        {
            PlayClipByName("card-spread", false, 0.5f);
        }

        public void PlaySodaOpen()
        {
            PlayClipByName("soda_open", false, 0.5f);
        }

        public void PlayFail()
        {
            PlayClipByName("fail", false, 0.5f);
        }

        public void PlayRandomCardShuffle()
        {
            int randomIndex = Random.Range(1, 5);
            PlayClipByName("card-shuffle" + randomIndex, false, 0.5f);
        }

        public void PlayClipByName(string clipName, bool loop = false, float volume = 1f, float pitch = 1f)
        {
            for (int i = 0; i < _clips.Length; i++)
            {
                if (_clips[i].name.Equals(clipName))
                {
                    StopClipByName(clipName);
                    PlayClip(_clips[i], loop, volume, pitch);
                    return;
                }
            }
        }

        public bool IsClipNamePlaying(string clipName)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if (_audioSources[i].clip != null && _audioSources[i].clip.name.Equals(clipName) && _audioSources[i].isPlaying)
                {
                    return true;
                }
            }
            return false;
        }

        public void StopClipByName(string clipName)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if (_audioSources[i].clip != null && _audioSources[i].clip.name.Equals(clipName))
                {
                    _audioSources[i].Stop();
                }
            }
        }

        public void StopClip(AudioClip clip)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if (_audioSources[i].clip != null && _audioSources[i].clip == clip)
                {
                    _audioSources[i].Stop();
                }
            }
        }

        public void PlayClip(AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    _audioSources[i].clip = clip;
                    _audioSources[i].loop = loop;
                    _audioSources[i].volume = volume;
                    _audioSources[i].pitch = pitch;
                    _audioSources[i].Play();
                    return;
                }
            }
        }

        public void PlayManualLoop(AudioClip clip, float delay, bool pitchDiff = true, bool randomDelay = false, float volume = 1f)
        {
            if (_manualLoopCo != null)
            {
                StopCoroutine(_manualLoopCo);
            }
            _manualLoopCo = StartCoroutine(ManualLoopProcess(clip, delay, pitchDiff, randomDelay, volume));
        }


        public void StopManualLoop()
        {
            if (_manualLoopCo != null)
            {
                StopCoroutine(_manualLoopCo);
            }
        }

        IEnumerator ManualLoopProcess(AudioClip clip, float delay, bool pitchDiff = true, bool randomDelay = false, float volume = 1f)
        {
            while (true)
            {
                PlayClip(clip, false, volume, pitchDiff == true ? Random.Range(0.9f, 1.1f) : 1f);
                //PlayClipByName(clipName, false, volume, pitchDiff == true ? Random.Range(0.9f, 1.1f) : 1f);
                if (randomDelay)
                {
                    yield return new WaitForSeconds(Random.Range(delay - 0.3f, delay + 0.3f));
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }

            }
        }

        public void StopClipAfter(string clipName, float delay)
        {
            StartCoroutine(StopClipAfterProcess(clipName, delay));
        }

        IEnumerator StopClipAfterProcess(string clipName, float delay)
        {
            yield return new WaitForSeconds(delay);
            StopClipByName(clipName);
        }

        private void OnDestroy()
        {
            StopManualLoop();
        }
    }
}