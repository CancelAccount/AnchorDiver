using System.Collections.Generic;
using Logger;
using MyGame.UI.HUD.Controller;
using UnityEngine;
using UnityEngine.UI;
using MyGame.Events;

namespace MyGame.UI.HUD.View
{
    /// <summary>
    /// HUD视图，负责显示锚数量等核心游戏信息
    /// 使用GridLayout + Image图标表示锚的可用状态
    /// </summary>
    public class HUDView : BaseView<HUDController>
    {
        [Header("锚数量显示")]
        [Tooltip("锚图标的父容器（GridLayoutGroup）")]
        [SerializeField] private Transform m_anchorIconContainer;

        [Tooltip("锚可用图标")]
        [SerializeField] private Sprite m_anchorAvailableIcon;

        [Tooltip("锚不可用（已用）图标")]
        [SerializeField] private Sprite m_anchorUsedIcon;

        [Header("氧气条")]
        [Tooltip("氧气条Slider")]
        [SerializeField] private UnityEngine.UI.Slider m_oxygenSlider;

        private List<Image> m_anchorIcons = new();
        private const string LOG_MODULE = LogModules.HUD;

        /// <summary>
        /// 初始化HUD
        /// </summary>
        protected override void Awake()
        {
            m_panelType = UIType.HUD;
            base.Awake();
        }

        private void Start()
        {
            GameEvents.TriggerMenuShow(UIType.HUD, true);
        }

        /// <summary>
        /// 尝试自动绑定控制器
        /// </summary>
        protected override void TryBindController()
        {
            if (TryGetComponent<HUDController>(out var controller))
            {
                controller.Initialize();
                controller.SetView(this);
                BindController(controller);
                return;
            }

            controller = gameObject.AddComponent<HUDController>();
            controller.Initialize();
            controller.SetView(this);
            BindController(controller);
        }

        /// <summary>
        /// 初始化锚图标列表（根据最大数量创建子对象）
        /// </summary>
        /// <param name="remaining">当前剩余数量</param>
        /// <param name="max">最大数量</param>
        public void InitializeAnchorIcons(int remaining, int max)
        {
            if (m_anchorIconContainer == null)
            {
                Log.Warning(LOG_MODULE, "锚图标容器未设置");
                return;
            }

            // 清除旧的图标
            foreach (var icon in m_anchorIcons)
            {
                if (icon != null) Destroy(icon.gameObject);
            }
            m_anchorIcons.Clear();

            // 创建图标
            for (int i = 0; i < max; i++)
            {
                GameObject iconObj = new($"AnchorIcon_{i}", typeof(Image));
                iconObj.transform.SetParent(m_anchorIconContainer, false);

                Image img = iconObj.GetComponent<Image>();
                img.sprite = (i < remaining) ? m_anchorAvailableIcon : m_anchorUsedIcon;
                img.preserveAspect = true;

                m_anchorIcons.Add(img);
            }

            Log.Info(LOG_MODULE, $"锚图标已初始化: {remaining}/{max}");
        }

        /// <summary>
        /// 更新锚图标显示
        /// </summary>
        /// <param name="remaining">当前剩余数量</param>
        /// <param name="max">最大数量</param>
        public void UpdateAnchorIcons(int remaining, int max)
        {
            // 数量变化时重新创建
            if (m_anchorIcons.Count != max)
            {
                InitializeAnchorIcons(remaining, max);
                return;
            }

            for (int i = 0; i < m_anchorIcons.Count; i++)
            {
                m_anchorIcons[i].sprite = (i < remaining) ? m_anchorAvailableIcon : m_anchorUsedIcon;
            }
        }

        /// <summary>
        /// 更新氧气条显示
        /// </summary>
        /// <param name="current">当前氧气值</param>
        /// <param name="max">最大氧气值</param>
        public void UpdateOxygenBar(float current, float max)
        {
            if (m_oxygenSlider != null)
            {
                m_oxygenSlider.maxValue = max;
                m_oxygenSlider.value = current;
            }
        }

        /// <summary>
        /// 显示HUD
        /// </summary>
        public override void Show()
        {
            Log.Info(LOG_MODULE, "显示HUD");
            base.Show();
        }

        /// <summary>
        /// 隐藏HUD
        /// </summary>
        public override void Hide()
        {
            Log.Info(LOG_MODULE, "隐藏HUD");
            base.Hide();
        }

        /// <summary>
        /// 清理HUD资源
        /// </summary>
        public override void Cleanup()
        {
            foreach (var icon in m_anchorIcons)
            {
                if (icon != null) Destroy(icon.gameObject);
            }
            m_anchorIcons.Clear();
            base.Cleanup();
        }
    }
}
