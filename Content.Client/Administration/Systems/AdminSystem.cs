using System.Linq;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared._Starlight.Administration.Events;
using Content.Shared.GameTicking;
using Content.Shared._Nix.Administration;
using Robust.Shared.Network;

namespace Content.Client.Administration.Systems
{
    public sealed partial class AdminSystem : EntitySystem
    {
        public event Action<List<PlayerInfo>>? PlayerListChanged;
        public event Action<StationEventsChangedEvent>? StationEventsChanged;
        public event Action<NixAdminPowerResultEvent>? NixAdminPowerResult;
        public event Action<NixAdminAudioResultEvent>? NixAdminAudioResult;
        public event Action<NixAdminAlertSnapshotEvent>? NixAdminAlertSnapshot;
        public event Action<NixAdminAlertResultEvent>? NixAdminAlertResult;
        public event Action<NixAdminSecureActionResultEvent>? NixAdminSecureActionResult;
        public event Action<NixAdminShuttleSnapshotEvent>? NixAdminShuttleSnapshot;
        public event Action<NixAdminShuttleResultEvent>? NixAdminShuttleResult;

        private Dictionary<NetUserId, PlayerInfo>? _playerList;
        public StationEventsChangedEvent? StationEventsSnapshot { get; private set; }
        public IReadOnlyList<PlayerInfo> PlayerList
        {
            get
            {
                if (_playerList != null) return _playerList.Values.ToList();

                return new List<PlayerInfo>();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeOverlay();
            SubscribeNetworkEvent<FullPlayerListEvent>(OnPlayerListChanged);
            SubscribeNetworkEvent<PlayerInfoChangedEvent>(OnPlayerInfoChanged);
            SubscribeNetworkEvent<StationEventsChangedEvent>(OnStationEventsChanged);
            SubscribeNetworkEvent<NixAdminPowerResultEvent>(ev => NixAdminPowerResult?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminAudioResultEvent>(ev => NixAdminAudioResult?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminAlertSnapshotEvent>(ev => NixAdminAlertSnapshot?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminAlertResultEvent>(ev => NixAdminAlertResult?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminSecureActionResultEvent>(ev => NixAdminSecureActionResult?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminShuttleSnapshotEvent>(ev => NixAdminShuttleSnapshot?.Invoke(ev));
            SubscribeNetworkEvent<NixAdminShuttleResultEvent>(ev => NixAdminShuttleResult?.Invoke(ev));
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ShutdownOverlay();
        }

        private void OnPlayerInfoChanged(PlayerInfoChangedEvent ev)
        {
            if(ev.PlayerInfo == null) return;

            if (_playerList == null) _playerList = new();

            _playerList[ev.PlayerInfo.SessionId] = ev.PlayerInfo;
            PlayerListChanged?.Invoke(_playerList.Values.ToList());
        }

        private void OnPlayerListChanged(FullPlayerListEvent msg)
        {
            _playerList = msg.PlayersInfo.ToDictionary(x => x.SessionId, x => x);
            PlayerListChanged?.Invoke(msg.PlayersInfo);
        }

        private void OnStationEventsChanged(StationEventsChangedEvent msg)
        {
            StationEventsSnapshot = msg;
            StationEventsChanged?.Invoke(msg);
        }

        public void RequestStationEvents()
        {
            RaiseNetworkEvent(new RequestStationEventsEvent());
        }

        public void SendStationEventCommand(
            StationEventQueueCommand command,
            string eventId = "",
            int queueId = 0,
            float seconds = -1f,
            NetEntity activeEvent = default)
        {
            RaiseNetworkEvent(new StationEventQueueCommandEvent
            {
                Command = command,
                EventId = eventId,
                QueueId = queueId,
                Seconds = seconds,
                ActiveEvent = activeEvent
            });
        }

        public void SendNixAdminAudioCommand(NixAdminAudioAction action, string path = "", byte volumePercent = 100)
        {
            RaiseNetworkEvent(new NixAdminAudioCommandEvent
            {
                Action = action,
                Path = path,
                VolumePercent = volumePercent
            });
        }

        public void SendNixAdminPowerCommand(NixAdminPowerAction action)
        {
            RaiseNetworkEvent(new NixAdminPowerCommandEvent { Action = action });
        }

        public void RequestNixAdminAlerts()
        {
            RaiseNetworkEvent(new NixAdminAlertRequestEvent());
        }

        public void SendNixAdminAlertCommand(NetEntity station, string level, bool locked)
        {
            RaiseNetworkEvent(new NixAdminSetAlertEvent
            {
                Station = station,
                Level = level,
                Locked = locked
            });
        }

        public void SendNixAdminSecureAction(string requestId)
        {
            RaiseNetworkEvent(new NixAdminSecureActionEvent { RequestId = requestId });
        }

        public void RequestNixAdminShuttle()
        {
            RaiseNetworkEvent(new NixAdminShuttleRequestEvent());
        }

        public void SendNixAdminShuttleCommand(
            NixAdminShuttleAction action,
            float seconds = 0f,
            string shuttlePath = "",
            bool locked = false)
        {
            RaiseNetworkEvent(new NixAdminShuttleCommandEvent
            {
                Action = action,
                Seconds = seconds,
                ShuttlePath = shuttlePath,
                Locked = locked
            });
        }
    }
}
