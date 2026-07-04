using Logger;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyGame.Item
{
    /// <summary>
    /// 涡流/水流区域
    /// 对进入区域的玩家施加指定方向的推力
    /// 挂载在Tilemap层上，配合TilemapCollider2D(IsTrigger=true)使用
    /// 每个方向一层：Current_Up / Current_Down / Current_Left / Current_Right
    /// </summary>
    [RequireComponent(typeof(TilemapCollider2D))]
    public class CurrentZone : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        [Header("涡流配置")]
        [Tooltip("推力方向")]
        [SerializeField] private Direction m_direction = Direction.Down;

        [Tooltip("推力大小")]
        [SerializeField] private float m_force = 5f;

        private Rigidbody2D m_playerRigidbody;
        private Vector2 m_forceVector;
        private const string LOG_MODULE = LogModules.PLAYER;

        private void Awake()
        {
            m_forceVector = m_direction switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => Vector2.down
            };
            m_forceVector *= m_force;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            m_playerRigidbody = other.GetComponent<Rigidbody2D>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            m_playerRigidbody = null;
        }

        private void FixedUpdate()
        {
            if (m_playerRigidbody != null)
            {
                m_playerRigidbody.AddForce(m_forceVector, ForceMode2D.Force);
            }
        }
    }
}
