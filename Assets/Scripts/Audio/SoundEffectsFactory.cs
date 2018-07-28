using System;
using System.Collections.Generic;
using svtz.Tanks.Common;
using svtz.Tanks.Infra;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Audio
{
    internal sealed class SoundEffectsFactory : MonoBehaviour
    {
#pragma warning disable 0649
        public AudioClip RegularShootClip;
        public AudioClip RegularWallDestroyClip;
        public AudioClip UnbreakableHitClip;
        public AudioClip BonusPickupClip;
        public AudioClip PlayerDeathClip;
        public AudioClip EnemyDeathClip;
        public AudioClip PlayerSpawnClip;
        public AudioClip GaussianChargeClip;
        public AudioClip GaussianShotClip;

        public AudioMixerGroup LocalPlayerMixerGroup;
        public AudioMixerGroup EnvironmentMixerGroup;
#pragma warning restore 0649

        internal sealed class SoundEffectSignal : Signal<SoundEffectSignal, SoundEffectSignal.Msg>
        {
            public class Msg : MessageBase
            {
                public Vector2 Position { get; set; }
                public SoundEffectKind Kind { get; set; }
                public SoundEffectSource Source { get; set; }

                public override void Serialize(NetworkWriter writer)
                {
                    base.Serialize(writer);
                    writer.Write(Position);
                    writer.Write(Kind.ToString());
                    writer.Write(Source.ToString());
                }

                public override void Deserialize(NetworkReader reader)
                {
                    base.Deserialize(reader);
                    Position = reader.ReadVector2();
                    Kind = (SoundEffectKind)Enum.Parse(typeof(SoundEffectKind), reader.ReadString());
                    Source = (SoundEffectSource)Enum.Parse(typeof(SoundEffectSource), reader.ReadString());
                }
            }

            internal sealed class ServerToClient : ServerToClientSignal<SoundEffectSignal, Msg>
            {
            }
        }

        private Dictionary<SoundEffectKind, AudioClip> AudioClips { get; set; }
        private Dictionary<SoundEffectSource, AudioMixerGroup> MixerGroups { get; set; }
        private Pool _pool;
        private DelayedExecutor _delayedExecutor;
        private SoundEffectSignal.ServerToClient _serverToClient;
        private SoundEffectSignal _soundEffectsSignal;

        private void Start()
        {
            AudioClips = new Dictionary<SoundEffectKind, AudioClip>
            {
                {SoundEffectKind.RegularShoot, RegularShootClip},
                {SoundEffectKind.RegularWallDestroy, RegularWallDestroyClip},
                {SoundEffectKind.UnbreakableHit, UnbreakableHitClip},
                {SoundEffectKind.BonusPickup, BonusPickupClip},
                {SoundEffectKind.PlayerDeath, PlayerDeathClip},
                {SoundEffectKind.EnemyDeath, EnemyDeathClip},
                {SoundEffectKind.PlayerSpawn, PlayerSpawnClip},
                {SoundEffectKind.GaussCharge, GaussianChargeClip},
                {SoundEffectKind.GaussShot, GaussianShotClip}
            };
            MixerGroups = new Dictionary<SoundEffectSource, AudioMixerGroup>
            {
                {SoundEffectSource.Environment, EnvironmentMixerGroup},
                {SoundEffectSource.LocalPlayer, LocalPlayerMixerGroup}
            };
        }

        [Inject]
        private void Construct(Pool pool,
            DelayedExecutor delayedExecutor,
            SoundEffectSignal.ServerToClient serverToClient,
            SoundEffectSignal soundEffectSignal)
        {
            _pool = pool;
            _delayedExecutor = delayedExecutor;
            _serverToClient = serverToClient;
            _soundEffectsSignal = soundEffectSignal;
            soundEffectSignal.Listen(Listener);
        }

        private void OnDestroy()
        {
            _soundEffectsSignal.Unlisten(Listener);
        }

        private void Listener(SoundEffectSignal.Msg obj)
        {
            Play(obj.Position, obj.Kind, obj.Source);
        }

        private float _currentTime = 0;

        private class Effect
        {
            public Vector2 Position;
            public SoundEffectKind Kind;
            public SoundEffectSource Source;
            public AudioSource AudioSource;
        }

        private readonly List<Effect> _currentEffects = new List<Effect>();

        private void PlayImpl(Vector2 position,
            SoundEffectKind soundEffectKind,
            SoundEffectSource soundEffectSource)
        {
            var clip = AudioClips[soundEffectKind];
            var effect = _pool.Spawn(clip, MixerGroups[soundEffectSource], position);
            var despawnAfter = clip.length * ((double)Time.timeScale >= 0.01f ? Time.timeScale : 0.01f);
            _delayedExecutor.Add(() => _pool.Despawn(effect), despawnAfter);

            _currentEffects.Add(new Effect
            {
                Kind = soundEffectKind,
                Position = position,
                Source = soundEffectSource,
                AudioSource = effect
            });
        }

        public void Play(Vector2 position,
            SoundEffectKind soundEffectKind,
            SoundEffectSource soundEffectSource)
        {
            if (Math.Abs(_currentTime - Time.time) > 0.01f)
            {
                _currentTime = Time.time;
                _currentEffects.Clear();
            }
            else
            {
                foreach (var effect in _currentEffects)
                {
                    if (effect.Kind != soundEffectKind)
                        continue;

                    if (effect.Position != position)
                    {
                        effect.Position = (effect.Position + position) / 2;
                        effect.AudioSource.transform.position = effect.Position;
                    }

                    if (effect.Source == SoundEffectSource.Environment &&
                        soundEffectSource == SoundEffectSource.LocalPlayer)
                    {
                        effect.Source = SoundEffectSource.LocalPlayer;
                        effect.AudioSource.outputAudioMixerGroup = MixerGroups[SoundEffectSource.LocalPlayer];
                    }

                    return;
                }

            }

            PlayImpl(position, soundEffectKind, soundEffectSource);
        }

        public void PlayOnAllClients(Vector2 position,
            SoundEffectKind soundEffectKind)
        {
            _serverToClient.FireOnAllClients(new SoundEffectSignal.Msg
            {
                Kind = soundEffectKind,
                Position = position,
                Source = SoundEffectSource.Environment
            });
        }

        public void PlayOnAllWithException(NetworkConnection toExcept,
            Vector2 position, SoundEffectKind soundEffectKind)
        {
            _serverToClient.FireOnAllWithException(toExcept, new SoundEffectSignal.Msg
            {
                Kind = soundEffectKind,
                Position = position,
                Source = SoundEffectSource.Environment
            });
        }

        public void PlayOnSingleClient(
            NetworkConnection client,
            Vector2 position,
            SoundEffectKind soundEffectKind)
        {
            _serverToClient.FireOnClient(client,
                new SoundEffectSignal.Msg
                {
                    Kind = soundEffectKind,
                    Position = position,
                    Source = SoundEffectSource.LocalPlayer
                });
        }

        public class Pool : MonoMemoryPool<AudioClip, AudioMixerGroup, Vector3, AudioSource>
        {
            protected override void Reinitialize(AudioClip p1, AudioMixerGroup p2, Vector3 p3, AudioSource item)
            {
                base.Reinitialize(p1, p2, p3, item);

                item.outputAudioMixerGroup = p2;
                item.transform.position = p3;
                item.PlayOneShot(p1);
            }

            protected override void OnCreated(AudioSource item)
            {
                base.OnCreated(item);

                item.playOnAwake = false;
                item.loop = false;
            }
        }
    }
}