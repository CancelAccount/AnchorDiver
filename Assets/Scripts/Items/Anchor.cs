using Logger;
using MyGame.Events;
using UnityEngine;
using MyGame.Data;


namespace MyGame.Item
{
    /// <summary>
    /// 船锚行为脚本
    /// 使用Rigidbody2D重力下落，CircleCast检测碰撞和碎石破坏
    /// 锚定后将Rigidbody2D设为Static停止
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Anchor : MonoBehaviour
    {
        [Header("重力配置")]
        [Tooltip("锚下落的重力倍率")]
        [SerializeField] private float m_gravityScale = 2f;

        [Header("碰撞配置")]
        [Tooltip("Collision检测半径")]
        [SerializeField] private float m_hitRadius = 0.3f;

        private Rigidbody2D m_rigidbody;
        private RubbleManager m_rubbleManager;
        private bool m_isAnchored;

        private const string LOG_MODULE = "Anchor";

        #region 生命周期

        /// <summary>
        /// 初始化组件引用和物理状态
        /// </summary>
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_rigidbody.gravityScale = m_gravityScale;
            m_rigidbody.bodyType = RigidbodyType2D.Dynamic;

            m_rubbleManager = FindFirstObjectByType<RubbleManager>();
        }

        /// <summary>
        /// 每帧检测下方碰撞
        /// </summary>
        private void Update()
        {
            if (m_isAnchored) return;

            CheckCollision();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// CircleCastAll检测下方碰撞，处理所有碎石破坏和地形锚定
        /// </summary>
        private void CheckCollision()
        {
            Vector2 origin = transform.position;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, m_hitRadius, Vector2.down, 0.1f,
                LayerMask.GetMask("Default"));

            if (hits.Length == 0) return;

            bool hitSolid = false;
            string solidName = "";

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                // 砸中玩家 → 死亡，锚带着尸体继续下落
                if (hit.collider.CompareTag("Player"))
                {
                    Log.Info(LOG_MODULE, "锚砸中玩家，触发死亡");
                    GameEvents.TriggerGameOver(false);
                    // 将玩家挂到锚下，随锚一起沉底
                    hit.collider.transform.SetParent(transform);
                    continue;
                }

                // 尝试销毁碎石
                if (m_rubbleManager != null)
                {
                    if (m_rubbleManager.DestroyTileAt(hit.point))
                    {
                        Log.Info(LOG_MODULE, "锚击碎碎石");
                        continue;
                    }
                }

                // 非碎石、非触发器的地形 → 锚停止
                if (!hit.collider.isTrigger)
                {
                    hitSolid = true;
                    solidName = hit.collider.name;
                }
                else
                {
                    // 触发器：主动触发开关（锚速度太快时物理事件可能漏掉）
                    var gateSwitch = hit.collider.GetComponent<GateSwitch>();
                    if (gateSwitch != null)
                    {
                        gateSwitch.TryTrigger(transform.position);
                    }
                }
            }

            if (hitSolid)
            {
                Log.Info(LOG_MODULE, $"锚命中地形: {solidName}");
                AnchorDown();
            }
        }

        /// <summary>
        /// 锚停止下落，设为Static固定在当前位置
        /// </summary>
        private void AnchorDown()
        {
            if (m_isAnchored) return;
            m_isAnchored = true;

            m_rigidbody.bodyType = RigidbodyType2D.Static;

            Log.Info(LOG_MODULE, "锚已锚定");
        }

        #endregion
    }
}
