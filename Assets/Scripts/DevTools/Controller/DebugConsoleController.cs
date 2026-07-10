using UnityEngine;
using Logger;
using MyGame.UI;

namespace MyGame.DevTool
{
    /// <summary>
    /// 调试控制台控制器，连接模型和视图，处理用户输入和命令执行
    /// </summary>
    public class DebugConsoleController : BaseController<DebugConsole, DebugCommandModel>
    {
        private const string LOG_MODULE = LogModules.DEBUGCONSOLE;

        /// <summary>
        /// 初始化MVC组件
        /// </summary>
        private void Awake()
        {
            InitializeMVCComponents();
            Initialize();
        }

        /// <summary>
        /// 初始化MVC组件
        /// </summary>
        private void InitializeMVCComponents()
        {
            // 创建并初始化Model
            CreateAndInitializeModel();

            // 初始化命令
            m_model.InitializeCommands();

            // 查找View引用
            if (m_view == null)
            {
                m_view = GetComponent<DebugConsole>();
            }

            if (m_view == null)
            {
                Log.Error(LOG_MODULE, "未找到DebugConsole组件");
            }
        }

        /// <summary>
        /// 处理用户输入的命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        public void HandleCommand(string commandText)
        {
            if (m_model.ExecuteCommand(commandText))
            {
                Log.Info(LOG_MODULE, "执行命令: " + commandText);
            }
            else if (!string.IsNullOrEmpty(commandText))
            {
                if (m_view != null)
                {
                    m_view.Print("未知命令，输入 help 查看可用命令。");
                }
                Log.Warning(LOG_MODULE, "未知命令: " + commandText);
            }
        }

        /// <summary>
        /// 输出信息到控制台
        /// </summary>
        /// <param name="message">消息内容</param>
        public void PrintToConsole(string message)
        {
            if (m_view != null)
            {
                m_view.Print(message);
            }
        }
    }
}
