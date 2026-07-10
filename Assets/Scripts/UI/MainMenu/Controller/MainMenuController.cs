using MyGame.Events;
using MyGame.Managers;
using MyGame.UI.MainMenu.Model;
using MyGame.UI.MainMenu.View;
using UnityEngine;

namespace MyGame.UI.MainMenu.Controller
{
    /// <summary>
    /// 主菜单控制器
    /// 连接模型和视图，处理业务逻辑，负责MVC组件的初始化和协调
    /// </summary>
    public class MainMenuController : BaseController<MainMenuView, MainMenuModel>
    {
        #region 字段

        [Header("菜单配置")]
        [Tooltip("默认启动的游戏场景名称")]
        [SerializeField] private string m_defaultGameScene = "GameLevel1";

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化控制器和MVC组件
        /// </summary>
        private void Awake()
        {
            InitializeMVCComponents();
            Initialize();
        }

        /// <summary>
        /// 启用时注册事件
        /// </summary>
        private void OnEnable()
        {
            RegisterEvents();
            BindModelEvents();
        }

        /// <summary>
        /// 禁用时注销事件
        /// </summary>
        private void OnDisable()
        {
            UnregisterEvents();
            UnbindModelEvents();
        }

        /// <summary>
        /// 销毁时清理资源
        /// </summary>
        private void OnDestroy()
        {
            UnregisterEvents();
            UnbindModelEvents();

            if (m_model != null)
            {
                m_model.Cleanup();
            }

            if (m_view != null)
            {
                m_view.UnbindController();
            }
        }

        #endregion

        #region MVC组件初始化

        /// <summary>
        /// 初始化MVC组件，建立模型和视图之间的连接
        /// </summary>
        private void InitializeMVCComponents()
        {
            // 创建并初始化Model
            CreateAndInitializeModel();

            // 设置默认游戏场景
            if (!string.IsNullOrEmpty(m_defaultGameScene))
            {
                m_model.DefaultGameScene = m_defaultGameScene;
            }

            // 查找或创建View
            if (m_view == null)
            {
                m_view = GetComponentInChildren<MainMenuView>(true);
                if (m_view == null)
                {
                    Debug.LogWarning("MainMenuController: MainMenuView not found, creating.");
                    GameObject viewObject = new("MainMenuView");
                    viewObject.transform.SetParent(transform, false);
                    m_view = viewObject.AddComponent<MainMenuView>();
                }
            }
        }

        #endregion

        #region Model事件绑定

        /// <summary>
        /// 订阅Model的属性变更事件
        /// </summary>
        private void BindModelEvents()
        {
            if (m_model != null)
            {
                m_model.OnPropertyChanged += HandleModelPropertyChanged;
            }
        }

        /// <summary>
        /// 取消订阅Model的属性变更事件
        /// </summary>
        private void UnbindModelEvents()
        {
            if (m_model != null)
            {
                m_model.OnPropertyChanged -= HandleModelPropertyChanged;
            }
        }

        /// <summary>
        /// Model属性变更回调
        /// 主菜单暂无View更新需求，但订阅以遵循ObservableModel规范
        /// </summary>
        private void HandleModelPropertyChanged(string propertyName)
        {
            // 主菜单的Model属性变更（如 IsSettingsVisible）由GameEvents驱动，
            // 暂不需要额外的View更新。保留订阅以满足ObservableModel使用规范。
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void OnStartGame()
        {
            GameEvents.TriggerSceneLoadStart("Level Select");
        }

        /// <summary>
        /// 显示设置面板
        /// </summary>
        public void OnShowSettings()
        {
            if (m_model != null)
            {
                m_model.IsSettingsVisible = true;
                GameEvents.TriggerMenuShow(UIType.SettingsPanel, true);
            }
        }

        /// <summary>
        /// 显示关于面板
        /// </summary>
        public void OnShowAbout()
        {
            if (m_model != null)
            {
                m_model.IsAboutVisible = true;
                GameEvents.TriggerMenuShow(UIType.AboutPanel, true);
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void OnExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion

        #region 事件注册

        /// <summary>
        /// 注册全局事件监听
        /// </summary>
        private void RegisterEvents()
        {
            GameEvents.OnGameStart += OnGameStart;
            GameEvents.OnSceneLoadComplete += OnSceneLoadComplete;
        }

        /// <summary>
        /// 注销全局事件监听
        /// </summary>
        private void UnregisterEvents()
        {
            GameEvents.OnGameStart -= OnGameStart;
            GameEvents.OnSceneLoadComplete -= OnSceneLoadComplete;
        }

        /// <summary>
        /// 游戏开始事件响应
        /// </summary>
        private void OnGameStart()
        {
            if (m_model != null)
            {
                SceneSwitcher.RequestLoadScene(m_model.DefaultGameScene);
            }
        }

        /// <summary>
        /// 场景加载完成事件响应
        /// </summary>
        /// <param name="sceneName">加载完成的场景名称</param>
        private void OnSceneLoadComplete(string sceneName)
        {
            if (sceneName == "MainMenu")
            {
                GameEvents.TriggerMenuShow(UIType.MainMenu, true);
            }
        }

        #endregion
    }
}
