using Logger;
using MyGame.Control;
using MyGame.Events;
using UnityEngine;
using MyGame.Data;

namespace MyGame.Item
{
    /// <summary>
    /// 船锚管理器
    /// 管理锚剩余次数、响应Interact键召唤锚、次数耗尽处理
    /// 挂载在关卡场景GameObject上（非Singleton）
    /// </summary>
    public class AnchorManager : MonoBehaviour
    {
        [Header("锚配置")]
        [Tooltip("锚预制体")]
        [SerializeField] private GameObject m_anchorPrefab;

        [Tooltip("最大锚数量（优先从关卡配置读取，此为编辑器直接测试时的兜底值）")]
        [SerializeField] private int m_defaultMaxAnchorCount = 2;

        [Tooltip("锚生成Y坐标（玩家正上方，应在地图顶部之外）")]
        [SerializeField] private float m_anchorSpawnY = 20f;

        [Header("玩家引用")]
        [Tooltip("玩家Transform（用于确定召唤位置）")]
        [SerializeField] private Transform m_playerTransform;

        private int m_maxAnchorCount;
        private int m_remainingAnchors;
        private bool m_hasActiveAnchor;
        private const string LOG_MODULE = "Anchor";

        #region 属性

        /// <summary>
        /// 剩余锚数量
        /// </summary>
        public int RemainingAnchors
        {
            get { return m_remainingAnchors; }
        }

        /// <summary>
        /// 最大锚数量
        /// </summary>
        public int MaxAnchorCount
        {
            get { return m_maxAnchorCount; }
        }

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化计数并绑定事件
        /// </summary>
        private void Start()
        {
            // 优先从关卡配置读取最大值，兜底使用Inspector默认值
            m_maxAnchorCount = LevelSession.MaxAnchors > 0 ? LevelSession.MaxAnchors : m_defaultMaxAnchorCount;
            m_remainingAnchors = m_maxAnchorCount;

            if (m_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    m_playerTransform = player.transform;
                }
            }

            Log.Info(LOG_MODULE, $"锚管理器初始化，剩余锚: {m_remainingAnchors}");
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 尝试召唤锚（由PlayerController的Interact键触发）
        /// </summary>
        /// <returns>是否成功召唤</returns>
        public bool TrySummonAnchor()
        {
            if (m_remainingAnchors <= 0)
            {
                Log.Warning(LOG_MODULE, "锚已耗尽，无法召唤");
                return false;
            }

            if (m_anchorPrefab == null)
            {
                Log.Error(LOG_MODULE, "锚预制体未设置");
                return false;
            }

            if (m_playerTransform == null)
            {
                Log.Error(LOG_MODULE, "玩家Transform未设置");
                return false;
            }

            // 锚生成在玩家正上方地图外
            Vector3 spawnPos = new(m_playerTransform.position.x, m_anchorSpawnY, 0f);
            Instantiate(m_anchorPrefab, spawnPos, Quaternion.identity);

            m_remainingAnchors--;
            m_hasActiveAnchor = true;

            Log.Info(LOG_MODULE, $"召唤锚！剩余: {m_remainingAnchors}/{m_maxAnchorCount}");

            // 通知HUD更新
            GameEvents.TriggerAnchorCountChanged(m_remainingAnchors, m_maxAnchorCount);

            return true;
        }

        /// <summary>
        /// 补充锚到上限：销毁场景中所有锚，重置剩余数量
        /// </summary>
        public void RefillAnchors()
        {
            // 销毁所有活跃的锚（先解除锚下挂载的子对象，避免玩家被连带销毁）
            Anchor[] anchors = FindObjectsOfType<Anchor>();
            foreach (var anchor in anchors)
            {
                // 解除锚下所有子对象（如被砸死的玩家）的父子关系
                for (int i = anchor.transform.childCount - 1; i >= 0; i--)
                {
                    anchor.transform.GetChild(i).SetParent(null);
                }
                Destroy(anchor.gameObject);
            }

            m_remainingAnchors = m_maxAnchorCount;
            m_hasActiveAnchor = false;

            Log.Info(LOG_MODULE, $"锚已补充至上限: {m_remainingAnchors}/{m_maxAnchorCount}");

            // 通知HUD更新
            GameEvents.TriggerAnchorCountChanged(m_remainingAnchors, m_maxAnchorCount);
        }

        #endregion
    }
}
