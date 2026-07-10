using Logger;
using MyGame.Events;
using MyGame.Managers;
using MyGame.UI.DeathOverlay.View;
using UnityEngine;

namespace MyGame.UI.DeathOverlay.Controller
{
    /// <summary>
    /// 死亡覆盖层控制器
    /// 监听游戏结束事件，失败时显示重开提示
    /// 此Controller无独立Model（仅有View显隐逻辑），使用非泛型BaseController
    /// </summary>
    public class DeathOverlayController : BaseController
    {
        #region MVC组件

        [Header("MVC组件")]
        [Tooltip("死亡覆盖层视图")]
        [SerializeField] private DeathOverlayView m_view;

        private const string LOG_MODULE = LogModules.DEATHOVERLAY;

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化视图引用并注册事件
        /// </summary>
        private void Awake()
        {
            InitializeView();
            Initialize();
        }

        /// <summary>
        /// 启用时注册事件
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnGameOver += OnGameOver;
            GameEvents.OnQuickRestart += OnQuickRestart;
        }

        /// <summary>
        /// 禁用时注销事件
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnGameOver -= OnGameOver;
            GameEvents.OnQuickRestart -= OnQuickRestart;
        }

        #endregion

        #region View初始化

        /// <summary>
        /// 初始化视图引用
        /// </summary>
        private void InitializeView()
        {
            if (m_view == null)
            {
                m_view = GetComponentInChildren<DeathOverlayView>(true);
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 游戏结束事件回调，失败时显示死亡面板
        /// </summary>
        /// <param name="isWin">是否胜利，false为死亡</param>
        private void OnGameOver(bool isWin)
        {
            if (isWin) return;

            Log.Info(LOG_MODULE, "玩家死亡，显示重开提示");

            Time.timeScale = 1f;

            if (InputManager.Instance != null)
            {
                InputManager.Instance.SwitchToUIMode();
            }

            if (m_view != null)
            {
                m_view.Show();
            }
        }

        /// <summary>
        /// 快速重开事件回调：隐藏死亡面板，切换回GamePlay模式
        /// </summary>
        private void OnQuickRestart()
        {
            Log.Info(LOG_MODULE, "快速重开，隐藏死亡面板");

            if (m_view != null)
            {
                m_view.Hide();
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.SwitchToGamePlayMode();
            }
        }

        #endregion
    }
}
