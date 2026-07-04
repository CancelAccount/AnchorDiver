using Logger;
using UnityEngine;

namespace MyGame.Item
{
    /// <summary>
    /// 涡流触发器
    /// 玩家进入时持续施加向下的拉力，与自然浮力形成对抗
    /// 挂载在 CurrentZone_Tilemap 层上（TilemapCollider2D 需 IsTrigger）
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CurrentZoneTrigger : MonoBehaviour
    {
        [Header("水流区参数")]
        [Tooltip("水流力度")]
        [SerializeField] private float m_pullForce = 15f;

        private const string LOG_MODULE = LogModules.PLAYER;

        /// <summary>
        /// 玩家在水流区区域内时每帧施加的力
        /// </summary>
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.AddForce(Vector2.down * m_pullForce);
            }
        }
    }
}
