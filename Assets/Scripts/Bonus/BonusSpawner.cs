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
            public float RespawnIfNotPickedSeconds;
            public float RespawnIfPickedSeconds;
#pragma warning restore 0649
        }

        private readonly DelayedExecutor _delayedExecutor;
        private readonly Settings _settings;
        private readonly BonusPool _pool;
        private readonly Dictionary<Vector2, DelayedExecutor.IDelayedTask> _respawnTasks
            = new Dictionary<Vector2, DelayedExecutor.IDelayedTask>();

        public BonusSpawner(DelayedExecutor delayedExecutor, Settings settings, BonusPool pool)
        {
            _delayedExecutor = delayedExecutor;
            _settings = settings;
            _pool = pool;
        }

        public void ServerAddSpawnPoint(float x, float y)
        {
            var position = new Vector2(x, y);
            ServerRespawnBonus(position);
        }

        private BonusKind GetBonusKind()
        {
            return BonusKind.MoveSpeedBoost;
        }

        public void ServerRespawnPicked(Bonus bonus)
        {
            var respawnTask = _respawnTasks[bonus.transform.position];
            respawnTask.TimeRemaining = _settings.RespawnIfPickedSeconds;
            _pool.Despawn(bonus);
        }

        private void ServerRespawnBonus(Vector2 position)
        {
            var bonus = _pool.GetOrSpawn(position);
            bonus.ServerChangeBonusKind(GetBonusKind());

            var respawnTask = _delayedExecutor.Add(() => ServerRespawnBonus(position), _settings.RespawnIfNotPickedSeconds);
            _respawnTasks[position] = respawnTask;
        }
    }
}