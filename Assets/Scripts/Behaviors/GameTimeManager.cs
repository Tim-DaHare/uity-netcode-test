using Classes;
using Unity.Netcode;
using UnityEngine;

namespace Behaviors
{
    public class GameTimeManager : NetworkBehaviour
    {
        [SerializeField] private float dayDuration = 30f;
        [SerializeField] private Light directionalLight;
        
        private static LobbyManager LobbyManager => GameManger.Singleton.LobbyManager;
        
        public bool IsNight => CurrentDaytime >= dayDuration * 0.5f;
        
        /// <summary>
        /// The Time of seconds since the match has started
        /// </summary>
        public float MatchTime => NetworkManager.ServerTime.TimeAsFloat - LobbyManager.MatchStartedAt;
        public float CurrentDaytime => MatchTime % dayDuration;
        public float DayTimeRatio => (float)CurrentDaytime / dayDuration;

        private void Update()
        {
            if (!LobbyManager.IsMatchStarted) return;

            directionalLight.transform.eulerAngles = new Vector3(360 * DayTimeRatio, 0, 0);
        }
    }
}
