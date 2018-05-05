using System.Linq;
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

        private void RefreshStatsScreen(BattleStatsUpdateSignal.Msg msg)
        {
            var stats = msg.BattleStats.Stats;

            var positions = Enumerable.Range(1, stats.Count).Select(i => i.ToString()).ToArray();
            FillColumn(PositionColumn, positions, CellPrefab);

            var orderedStats = stats.Values
                .OrderByDescending(s => s.Frags)
                .ThenBy(s => s.Deaths)
                .ToArray();

            var names = orderedStats.Select(s => s.Name).ToArray();
            FillColumn(NameColumn, names, FlexibleCellPrefab);

            var deaths = orderedStats.Select(s => s.Deaths.ToString()).ToArray();
            FillColumn(DeathsColumn, deaths, CellPrefab);

            var frags = orderedStats.Select(s => s.Frags.ToString()).ToArray();
            FillColumn(FragsColumn, frags, CellPrefab);
        }

        private static void FillColumn(RectTransform column, string[] values, GameObject cellPrefab)
        {
            var transform = column.transform;

            // удалим лишние ячейки, если они есть
            // учитываем, что есть ещё заголовочная ячейка, которую трогать не нужно
            for (var i = values.Length; i < transform.childCount - 1; ++i)
            {
                var child = transform.GetChild(i + 1);
                Destroy(child.gameObject);
            }

            // добавим недостающие ячейки, если они есть
            // учитываем, что есть ещё заголовочная ячейка, которую трогать не нужно
            for (var i = transform.childCount - 1; i < values.Length; ++i)
            {
                Instantiate(cellPrefab, transform);
            }

            for (var i = 1; i <= values.Length; i++)
            {
                var textComponent = transform.GetChild(i).GetComponentInChildren<Text>();
                textComponent.text = values[i -1];
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