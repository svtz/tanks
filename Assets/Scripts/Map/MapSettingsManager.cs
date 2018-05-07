using System;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapSettingsManager : IMapSettings
    {
        public float BackgroundWidth { get; private set; }
        public float BackgroundHeight { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color CrawlerBeltColor { get; private set; }
        public event EventHandler Updated;

        private MapSettingsUpdatedSignal.ServerToClient _signalUpdatedSender;

        [Inject]
        private void Construct(MapSettingsUpdatedSignal signalUpdated,
            MapSettingsUpdatedSignal.ServerToClient signalUpdatedSender)
        {
            _signalUpdatedSender = signalUpdatedSender;
            signalUpdated.Listen(ClientUpdate);
        }

        private void ClientUpdate(MapSettingsUpdatedSignal.Msg obj)
        {
            BackgroundWidth = obj.BackgroundWidth;
            BackgroundHeight = obj.BackgroundHeight;
            BackgroundColor = obj.BackgroundColor;
            CrawlerBeltColor = obj.CrawlerBeltColor;
            OnUpdated();
        }

        public void ServerUpdate(MapInfo info)
        {
            // +1.5, т.к. есть периметр из неразрушимых блоков
            BackgroundWidth = info.Width + 1.5f;
            BackgroundHeight = info.Height + 1.5f;
            BackgroundColor = info.BackgroundColor;
            CrawlerBeltColor = info.CrawlerBeltColor;

            var msg = new MapSettingsUpdatedSignal.Msg
            {
                BackgroundWidth = BackgroundWidth,
                BackgroundHeight = BackgroundHeight,
                BackgroundColor = BackgroundColor,
                CrawlerBeltColor = CrawlerBeltColor
            };

            _signalUpdatedSender.FireOnAllClients(msg);
        }

        private void OnUpdated()
        {
            var handler = Updated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}