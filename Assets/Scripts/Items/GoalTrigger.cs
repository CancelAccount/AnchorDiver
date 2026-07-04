using Logger;
using MyGame.Events;
using UnityEngine;

namespace MyGame.Item
{
    /// <summary>
    /// 出口/宝藏触发器
    /// 玩家触碰时触发胜利，挂载在出口Tilemap层上
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GoalTrigger : MonoBehaviour
    {
        private const string LOG_MODULE = LogModules.PLAYER;

        /// <summary>
        /// 碰撞检测，玩家可触发
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Log.Info(LOG_MODULE, "玩家到达出口，触发胜利");
                GameEvents.TriggerGameOver(true);
            }
        }
    }
}
