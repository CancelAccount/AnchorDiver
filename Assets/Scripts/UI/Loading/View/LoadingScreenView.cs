using System.Collections;
using Logger;
using UnityEngine;

using MyGame.UI.Loading.Controller;

namespace MyGame.UI.Loading.View
{
    /// <summary>
    /// 加载界面组件
    /// MVC架构中的View层，负责显示加载界面的UI元素和动画效果
    /// 使用Animator.Play()直接控制动画，不再依赖Animation Event
    /// </summary>
    public class LoadingScreenView : BaseView<LoadingScreenController>
    {
        private const string LOG_MODULE = LogModules.LOADING;
        
        // 动画状态名称常量（与Animator Controller中State名称一致）
        private const string STATE_SHOW_LOADING = "ShowLoading";
        private const string STATE_HIDE_LOADING = "HideLoading";

        [Header("层级设置")]
        [Tooltip("加载界面Canvas的Sorting Order。值越高，显示层级越高，不易被其他UI遮挡。")]
        public int canvasSortingOrder = 1000;
        
        [Header("动画设置")]
        [Tooltip("控制加载界面显隐动画的Animator组件")]
        [SerializeField] private Animator m_animator;

        /// <summary>
        /// 隐藏动画协程引用，用于防止重复启动
        /// </summary>
        private Coroutine m_hideCoroutine;

        /// <summary>
        /// 初始化加载界面
        /// </summary>
        protected override void Awake()
        {
            // 设置面板类型为Loading
            m_panelType = UIType.Loading;
            
            // 调用基类的Awake方法，完成基础初始化
            base.Awake();
            
            // 确保使用全局Canvas
            EnsureGlobalCanvasParent();
            
            // 自动获取Animator组件
            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
                if (m_animator == null)
                {
                    m_animator = gameObject.AddComponent<Animator>();
                    Log.Info(LOG_MODULE, "已自动添加Animator组件", this);
                }
            }
        }
        
        /// <summary>
        /// 确保加载界面使用全局Canvas作为父级并设置正确的排序层级
        /// </summary>
        private void EnsureGlobalCanvasParent()
        {
            // 获取全局Canvas
            GameObject globalCanvasObj = GameObject.Find("GlobalUI");
            Canvas globalCanvas = globalCanvasObj != null ? globalCanvasObj.GetComponent<Canvas>() : null;
            if (globalCanvas != null)
            {
                // 如果当前对象不在全局Canvas下，则将其移动到全局Canvas下
                if (transform.parent != globalCanvas.transform)
                {
                    transform.SetParent(globalCanvas.transform, false);
                }
                
                // 获取或添加面板自身的Canvas组件
                if (!TryGetComponent<Canvas>(out var panelCanvas))
                {
                    panelCanvas = gameObject.AddComponent<Canvas>();
                    panelCanvas.overrideSorting = true;
                }
                
                // 设置Canvas的排序层级为1000，与PanelLoader中设置的Loading类型排序层级保持一致
                panelCanvas.sortingOrder = canvasSortingOrder;
                Log.Info(LOG_MODULE, $"已设置加载界面Canvas排序层级为: {canvasSortingOrder}", this);
            }
            else
            {
                Log.Warning(LOG_MODULE, "未找到全局Canvas组件。", this);
            }
        }
        
        /// <summary>
        /// 尝试自动绑定控制器
        /// 创建并绑定LoadingScreenController实例
        /// </summary>
        protected override void TryBindController()
        {
            LoadingScreenController controller = gameObject.AddComponent<LoadingScreenController>();
            
            // 初始化控制器
            controller.Initialize();
            
            // 设置控制器的视图引用
            controller.SetView(this);
            
            // 绑定控制器到视图
            BindController(controller);
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public override void Initialize()
        {
            // 可以在这里进行额外的初始化逻辑
        }
        
        /// <summary>
        /// 清理面板资源
        /// </summary>
        public override void Cleanup()
        {
            if (m_hideCoroutine != null)
            {
                StopCoroutine(m_hideCoroutine);
                m_hideCoroutine = null;
            }
            base.Cleanup();
        }
        
        /// <summary>
        /// 显示加载界面
        /// 每次调用都会强制激活GameObject并播放ShowLoading动画
        /// </summary>
        public override void Show()
        {
            // 确保GameObject激活
            gameObject.SetActive(true);
            
            // 确保CanvasGroup可见
            if (m_canvasGroup != null)
            {
                m_canvasGroup.alpha = 1f;
            }
            
            // 从第0帧播放ShowLoading动画
            if (m_animator != null)
            {
                m_animator.Play(STATE_SHOW_LOADING, 0, 0f);
            }
            
            IsVisible = true;
            Log.Info(LOG_MODULE, "加载界面显示，播放ShowLoading动画");
        }
        
        /// <summary>
        /// 隐藏加载界面
        /// 播放HideLoading动画，通过协程在动画结束后清理，不依赖Animation Event
        /// </summary>
        public override void Hide()
        {
            if (!IsVisible)
            {
                return;
            }
            
            Log.Info(LOG_MODULE, "隐藏加载界面，播放HideLoading动画");
            
            // 立即标记为不可见，确保下次Show()能正常执行
            IsVisible = false;
            
            if (m_animator != null)
            {
                // 从第0帧播放HideLoading动画
                m_animator.Play(STATE_HIDE_LOADING, 0, 0f);
                
                // 停止之前的协程（如果有）
                if (m_hideCoroutine != null)
                {
                    StopCoroutine(m_hideCoroutine);
                }
                
                // 启动协程等待动画结束后清理
                m_hideCoroutine = StartCoroutine(WaitForHideAnimationComplete());
            }
            else
            {
                // 无Animator时直接完成清理
                CompleteHideCleanup();
            }
        }
        
        /// <summary>
        /// 等待HideLoading动画播放完成，然后执行清理
        /// </summary>
        private IEnumerator WaitForHideAnimationComplete()
        {
            // 等待一帧确保Animator状态已更新到HideLoading
            yield return null;
            
            // 获取HideLoading动画的实际长度
            float animLength = 1f; // 默认兜底值
            if (m_animator != null)
            {
                AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(STATE_HIDE_LOADING))
                {
                    animLength = stateInfo.length;
                }
            }
            
            // 等待动画播放完成（使用unscaled时间，确保Time.timeScale=0时也能完成）
            yield return new WaitForSecondsRealtime(animLength);
            
            CompleteHideCleanup();
            m_hideCoroutine = null;
        }
        
        /// <summary>
        /// 隐藏动画完成后的清理逻辑
        /// 将GameObject设为inactive，通知Controller
        /// </summary>
        private void CompleteHideCleanup()
        {
            Log.Info(LOG_MODULE, "加载界面隐藏动画完成，执行清理");
            
            // 通知控制器（控制器会将GameObject设为inactive）
            if (m_controller != null)
            {
                m_controller.OnHideAnimationComplete();
            }
        }
    
    }
}
