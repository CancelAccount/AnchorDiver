using Logger;
using MyGame.Events;
using MyGame.Item;
using MyGame.UI.HUD.Model;
using MyGame.UI.HUD.View;
using MyGame.Data;
using UnityEngine;

namespace MyGame.UI.HUD.Controller
{
    /// <summary>
    /// HUD控制器，负责处理HUD逻辑和事件响应
    /// </summary>
    public class HUDController : BaseController<HUDView, HUDModel>
    {
        private const string LOG_MODULE = LogModules.HUD;

        /// <summary>
        /// 初始化控制器
        /// </summary>
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                Log.Info(LOG_MODULE, "初始化HUD控制器");

                CreateAndInitializeModel();
                base.Initialize();
            }
        }

        /// <summary>
        /// 初始化逻辑，注册事件监听
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            RegisterEvents();
        }

        /// <summary>
        /// 清理逻辑，注销事件监听
        /// </summary>
        protected override void OnCleanup()
        {
            UnregisterEvents();
            base.OnCleanup();
        }

        /// <summary>
        /// 注册事件监听
        /// </summary>
        private void RegisterEvents()
        {
            GameEvents.OnAnchorCountChanged += OnAnchorCountChanged;
            GameEvents.OnOxygenChanged += OnOxygenChanged;
        }

        /// <summary>
        /// 注销事件监听
        /// </summary>
        private void UnregisterEvents()
        {
            GameEvents.OnAnchorCountChanged -= OnAnchorCountChanged;
            GameEvents.OnOxygenChanged -= OnOxygenChanged;
        }

        /// <summary>
        /// 设置视图引用并初始化锚图标
        /// </summary>
        public override void SetView(HUDView view)
        {
            base.SetView(view);
            if (m_view != null)
            {
                int maxAnchors = GetMaxAnchorCount();
                m_view.InitializeAnchorIcons(maxAnchors, maxAnchors);
            }
        }

        /// <summary>
        /// 获取最大锚数量（优先从关卡配置读取，兜底从AnchorManager读取）
        /// </summary>
        private int GetMaxAnchorCount()
        {
            if (LevelSession.MaxAnchors > 0) return LevelSession.MaxAnchors;

            var anchorManager = Object.FindObjectOfType<AnchorManager>();
            if (anchorManager != null) return anchorManager.MaxAnchorCount;

            Log.Warning(LOG_MODULE, "未找到关卡配置或AnchorManager，使用默认锚数量: 2");
            return 2;
        }

        /// <summary>
        /// 锚数量变更回调，通知视图更新
        /// </summary>
        /// <param name="remaining">剩余数量</param>
        /// <param name="max">最大数量</param>
        private void OnAnchorCountChanged(int remaining, int max)
        {
            if (m_view != null)
            {
                m_view.UpdateAnchorIcons(remaining, max);
            }
        }

        /// <summary>
        /// 氧气值变更回调，通知视图更新氧气条
        /// </summary>
        /// <param name="current">当前氧气值</param>
        /// <param name="max">最大氧气值</param>
        private void OnOxygenChanged(float current, float max)
        {
            if (m_view != null)
            {
                m_view.UpdateOxygenBar(current, max);
            }
        }
    }
}
