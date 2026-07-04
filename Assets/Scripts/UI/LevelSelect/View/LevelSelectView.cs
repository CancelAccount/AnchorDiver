using System.Collections.Generic;
using Logger;
using MyGame.Data;
using MyGame.UI.LevelSelect.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.UI.LevelSelect.View
{
    /// <summary>
    /// 选关界面视图
    /// 负责显示关卡按钮列表和返回按钮
    /// </summary>
    public class LevelSelectView : BaseView<LevelSelectController>
    {
        #region UI组件

        [Header("布局容器")]
        [Tooltip("关卡按钮的父容器")]
        [SerializeField] private Transform m_levelButtonContainer;

        [Header("按钮预制体")]
        [Tooltip("单个关卡按钮的预制体")]
        [SerializeField] private GameObject m_levelButtonPrefab;

        [Header("控制按钮")]
        [Tooltip("返回主菜单按钮")]
        [SerializeField] private Button m_backButton;

        private const string LOG_MODULE = LogModules.LEVELSELECT;

        #endregion

        #region 内部状态

        /// <summary>
        /// 当前生成的关卡按钮列表
        /// </summary>
        private List<Button> m_levelButtons = new();

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化面板
        /// </summary>
        protected override void Awake()
        {
            m_panelType = UIType.LevelSelect;
            base.Awake();
            BindButtonEvents();
        }

        /// <summary>
        /// 尝试自动绑定控制器
        /// </summary>
        protected override void TryBindController()
        {
            if (transform.parent != null && transform.parent.TryGetComponent<LevelSelectController>(out var controller))
            {
                BindController(controller);
                return;
            }

            controller = GetComponentInParent<LevelSelectController>();
            if (controller == null)
            {
                controller = gameObject.AddComponent<LevelSelectController>();
            }
            BindController(controller);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 根据关卡数据列表生成按钮
        /// </summary>
        /// <param name="levels">关卡数据列表</param>
        public void PopulateLevelButtons(List<LevelData> levels)
        {
            ClearLevelButtons();

            if (levels == null || levels.Count == 0)
            {
                Log.Warning(LOG_MODULE, "关卡列表为空，跳过按钮生成");
                return;
            }

            if (m_levelButtonPrefab == null)
            {
                Log.Error(LOG_MODULE, "关卡按钮预制体未设置");
                return;
            }

            for (int i = 0; i < levels.Count; i++)
            {
                int index = i;
                LevelData level = levels[i];

                GameObject buttonObj = Instantiate(m_levelButtonPrefab, m_levelButtonContainer);
                buttonObj.name = $"LevelButton_{level.levelId}";

                if (buttonObj.TryGetComponent<Button>(out var button))
                {
                    m_levelButtons.Add(button);

                    // 锁定关卡不可交互
                    if (level.IsUnlocked)
                    {
                        button.interactable = true;
                        button.onClick.AddListener(() => OnLevelButtonClicked(index));
                    }
                    else
                    {
                        button.interactable = false;
                    }
                }

                // 设置按钮上的文本（兼容 UGUI Text 和 TMP TextMeshProUGUI）
                SetButtonText(buttonObj, level.levelName);
            }

            Log.Info(LOG_MODULE, $"已生成 {m_levelButtons.Count} 个关卡按钮");
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        public override void Show()
        {
            base.Show();
            Log.Info(LOG_MODULE, "显示选关面板");
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public override void Hide()
        {
            base.Hide();
        }

        /// <summary>
        /// 清理面板资源
        /// </summary>
        public override void Cleanup()
        {
            UnbindButtonEvents();
            ClearLevelButtons();
            base.Cleanup();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绑定控制按钮事件
        /// </summary>
        private void BindButtonEvents()
        {
            if (m_backButton != null)
                m_backButton.onClick.AddListener(OnBackButtonClicked);
        }

        /// <summary>
        /// 解绑控制按钮事件
        /// </summary>
        private void UnbindButtonEvents()
        {
            if (m_backButton != null)
                m_backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        /// <summary>
        /// 清除已生成的关卡按钮
        /// </summary>
        private void ClearLevelButtons()
        {
            foreach (var button in m_levelButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            m_levelButtons.Clear();
        }

        /// <summary>
        /// 设置按钮文本（兼容 UGUI Text 和 TMP TextMeshProUGUI）
        /// </summary>
        /// <param name="buttonObj">按钮GameObject</param>
        /// <param name="text">文本内容</param>
        private void SetButtonText(GameObject buttonObj, string text)
        {
            // 优先尝试 TMP 文本组件
            var tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = text;
                return;
            }

            // 回退到 UGUI Text 组件
            var uguiText = buttonObj.GetComponentInChildren<Text>();
            if (uguiText != null)
            {
                uguiText.text = text;
            }
        }

        /// <summary>
        /// 关卡按钮点击回调，直接进入对应关卡
        /// </summary>
        /// <param name="index">按钮索引</param>
        private void OnLevelButtonClicked(int index)
        {
            if (m_controller != null)
            {
                m_controller.OnLevelButtonClicked(index);
            }
        }

        /// <summary>
        /// 返回按钮点击回调
        /// </summary>
        private void OnBackButtonClicked()
        {
            if (m_controller != null)
            {
                m_controller.OnBackToMainMenu();
            }
        }

        #endregion
    }
}
