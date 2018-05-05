using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ModestTree.Util;
using svtz.Tanks.BattleStats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class InGameGUIState : AbstractGUIState
    {
#pragma warning disable 0649
        public RectTransform StatsScreen;
        public RectTransform PositionColumn;
        public RectTransform NameColumn;
        public RectTransform DeathsColumn;
        public RectTransform FragsColumn;

        public GameObject FlexibleCellPrefab;
        public GameObject CellPrefab;
#pragma warning restore 0649

        public override GUIState Key
        {
            get { return GUIState.InGame; }
        }

        [Inject]
        public void Construct(BattleStatsUpdateSignal battleStatsUpdateSignal)
        {
            battleStatsUpdateSignal.Listen(RefreshStatsScreen);
        }

        private static void AddOrCreateCellsIfNeeded(RectTransform column, GameObject cellPrefab, int count)
        {
            var transform = column.transform;

            // удалим лишние ячейки, если они есть
            // учитываем, что есть ещё заголовочная ячейка, которую трогать не нужно
            for (var i = count; i < transform.childCount - 1; ++i)
            {
                var child = transform.GetChild(i + 1);
                Destroy(child.gameObject);
            }

            // добавим недостающие ячейки, если они есть
            // учитываем, что есть ещё заголовочная ячейка, которую трогать не нужно
            for (var i = transform.childCount - 1; i < count; ++i)
            {
                Instantiate(cellPrefab, transform);
            }
        }

        private static void SetValue(RectTransform column, int rowNum, string value, bool highlight)
        {
            var cell = column.transform.GetChild(rowNum).GetComponent<StatCellController>();
            cell.SetText(value, highlight);
        }

        private void RefreshStatsScreen(BattleStatsUpdateSignal.Msg msg)
        {
            var stats = msg.BattleStats.Stats;

            AddOrCreateCellsIfNeeded(PositionColumn, CellPrefab, stats.Count);
            AddOrCreateCellsIfNeeded(NameColumn, FlexibleCellPrefab, stats.Count);
            AddOrCreateCellsIfNeeded(DeathsColumn, CellPrefab, stats.Count);
            AddOrCreateCellsIfNeeded(FragsColumn, CellPrefab, stats.Count);

            var orderedStats = stats.Values
                .OrderByDescending(s => s.Frags)
                .ThenBy(s => s.Deaths);

            using (var enumerator = orderedStats.GetEnumerator())
            {
                var num = 1;
                while (enumerator.MoveNext())
                {
                    var stat = enumerator.Current;
                    var highlight = stat.Id == msg.PlayerId;

                    SetValue(PositionColumn, num, num.ToString(), highlight);
                    SetValue(NameColumn, num, stat.Name, highlight);
                    SetValue(DeathsColumn, num, stat.Deaths.ToString(), highlight);
                    SetValue(FragsColumn, num, stat.Frags.ToString(), highlight);

                    num++;
                }
            }
        }


        private bool _statsVisible = false;
        protected override void Update()
        {
            base.Update();

            var showStats = Input.GetKey(KeyCode.Tab);
            if (showStats ^ _statsVisible) // если различаются - надо синхронизировать
            {
                StatsScreen.gameObject.SetActive(showStats);
                _statsVisible = showStats;
            }
        }

        protected override void OnEscape()
        {
            GoToState(GUIState.GameMenu);
        }
    }
}