using System;
using Unity.Netcode;
using UnityEngine;

namespace Behaviors
{
    public class GameTimeManager : NetworkBehaviour
    {
        [SerializeField] private float dayDuration = 30f;
        [SerializeField] private Light directionalLight;
        
        public event Action OnNightTimeStart;
        public event Action OnNightTimeEnd;
        private static LobbyManager LobbyManager => GameManger.Singleton.LobbyManager;
        
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsNight => DayTimeRatio > 0.5f;
        public bool IsDay => !IsNight;
        
        /// <summary>
        /// The Time in seconds since the match has started
        /// </summary>
        public float MatchTime => NetworkManager.ServerTime.TimeAsFloat - LobbyManager.MatchStartedAt;
        public float CurrentDaytime => MatchTime % dayDuration;
        public float DayTimeRatio => CurrentDaytime / dayDuration;
        
        private float _prevDayRatio;
        
        private void Update()
        {
            if (!LobbyManager.IsMatchStarted) return;
            
            // it became nighttime this frame, raise events
            if (_prevDayRatio < 0.5f && DayTimeRatio >= 0.5f)
                OnNightTimeStart?.Invoke();

            // nighttime ends this frame, raise events
            if (_prevDayRatio > 0.5f && DayTimeRatio < 0.5f)
                OnNightTimeEnd?.Invoke();
            
            _prevDayRatio = DayTimeRatio;
            
            directionalLight.transform.eulerAngles = new Vector3(360 * DayTimeRatio, 0, 0);
        }
    }
}
