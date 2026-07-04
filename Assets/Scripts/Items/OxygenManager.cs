using Logger;
using MyGame.Events;
using UnityEngine;
using MyGame.Data;

namespace MyGame.Item
{
    /// <summary>
    /// 氧气管理器
    /// 管理玩家氧气值、随时间消耗、耗尽触发死亡、重生/存档点补充氧气
    /// 挂载在关卡场景GameObject上（非Singleton）
    /// </summary>
    public class OxygenManager : MonoBehaviour
    {
        [Header("氧气配置")]
        [Tooltip("最大氧气值（优先从关卡配置读取，此为编辑器直接测试时的兜底值）")]
        [SerializeField] private float m_defaultMaxOxygen = 30f;

        [Tooltip("每秒消耗量")]
        [SerializeField] private float m_depletionRate = 1f;

        [Tooltip("死亡后是否继续计时")]
        [SerializeField] private bool m_depleteWhenDead = false;

        private float m_maxOxygen;
        private float m_currentOxygen;
        private bool m_isDead;
        private const string LOG_MODULE = LogModules.PLAYER;

        #region 属性

        public float CurrentOxygen => m_currentOxygen;
        public float MaxOxygen => m_maxOxygen;
        public float OxygenPercent => m_currentOxygen / m_maxOxygen;

        #endregion

        #region 生命周期

        private void Awake()
        {
            // 优先从关卡配置读取最大值，兜底使用Inspector默认值
            m_maxOxygen = LevelSession.MaxOxygen > 0f ? LevelSession.MaxOxygen : m_defaultMaxOxygen;
            m_currentOxygen = m_maxOxygen;
            GameEvents.OnGameOver += OnGameOver;
            GameEvents.OnQuickRestart += OnQuickRestart;
        }

        private void Start()
        {
            // 初始触发一次，通知HUD当前氧气值
            GameEvents.TriggerOxygenChanged(m_currentOxygen, m_maxOxygen);
        }

        private void Update()
        {
            if (m_isDead && !m_depleteWhenDead) return;
            if (m_currentOxygen <= 0f) return;

            m_currentOxygen -= m_depletionRate * Time.deltaTime;

            if (m_currentOxygen <= 0f)
            {
                m_currentOxygen = 0f;
                GameEvents.TriggerOxygenChanged(m_currentOxygen, m_maxOxygen);
                OnOxygenDepleted();
            }
            else
            {
                GameEvents.TriggerOxygenChanged(m_currentOxygen, m_maxOxygen);
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnGameOver -= OnGameOver;
            GameEvents.OnQuickRestart -= OnQuickRestart;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 死亡时停止消耗
        /// </summary>
        private void OnGameOver(bool isWin)
        {
            m_isDead = true;
        }

        /// <summary>
        /// 快速重开/重生时恢复氧气
        /// </summary>
        private void OnQuickRestart()
        {
            m_isDead = false;
            RefillOxygen();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 补充氧气至满
        /// </summary>
        public void RefillOxygen()
        {
            m_currentOxygen = m_maxOxygen;
            GameEvents.TriggerOxygenChanged(m_currentOxygen, m_maxOxygen);
            Log.Info(LOG_MODULE, $"氧气已补充至上限: {m_maxOxygen}");
        }

        /// <summary>
        /// 添加指定量氧气（不超过上限）
        /// </summary>
        /// <param name="amount">补充量</param>
        public void AddOxygen(float amount)
        {
            m_currentOxygen = Mathf.Min(m_currentOxygen + amount, m_maxOxygen);
            GameEvents.TriggerOxygenChanged(m_currentOxygen, m_maxOxygen);
            Log.Info(LOG_MODULE, $"氧气恢复 {amount}，当前: {m_currentOxygen}/{m_maxOxygen}");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 氧气耗尽，触发死亡
        /// </summary>
        private void OnOxygenDepleted()
        {
            m_isDead = true;
            Log.Info(LOG_MODULE, "氧气耗尽，触发死亡");
            GameEvents.TriggerGameOver(false);
        }

        #endregion
    }
}
