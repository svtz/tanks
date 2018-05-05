using System;
using System.Collections.Generic;
using svtz.Tanks.Common;
using UnityEngine;

namespace svtz.Tanks.Bonus
{
    internal sealed class BonusSpawner
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public float RespawnSeconds;
#pragma warning restore 0649
        }

        private readonly DelayedExecutor _delayedExecutor;
        private readonly Settings _settings;
        private readonly BonusPool _pool;
        private List<Vector2> _points = new List<Vector2>();


        public BonusSpawner(DelayedExecutor delayedExecutor, Settings settings, BonusPool pool)
        {
            _delayedExecutor = delayedExecutor;
            _settings = settings;
            _pool = pool;
        }

        public void AddSpawnPoint(float x, float y)
        {
            var position = new Vector2(x, y);
            _points.Add(position);
            _delayedExecutor.Add(() => RespawnBonus(position), _settings.RespawnSeconds);
        }

        private BonusKind GetBonusKind()
        {
            return BonusKind.MoveSpeedBoost;
        }

        private void RespawnBonus(Vector2 position)
        {
            var bonus = _pool.GetOrSpawn(position);
        }
    }

    internal enum BonusKind
    {
        MoveSpeedBoost
    }
}