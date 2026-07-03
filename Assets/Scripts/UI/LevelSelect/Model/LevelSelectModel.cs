using System.Collections.Generic;
using MyGame.Data;
using MyGame.Events;
using MyGame.Managers;
using UnityEngine;

namespace MyGame.UI.LevelSelect.Model
{
    /// <summary>
    /// 选关界面的数据模型
    /// 管理关卡列表和选中状态
    /// </summary>
    public class LevelSelectModel : ObservableModel
    {
        #region 字段

        private List<LevelData> m_levels = new();
        private int m_selectedLevelIndex = -1;

        #endregion

        #region 属性

        /// <summary>
        /// 可选的关卡列表
        /// </summary>
        public List<LevelData> Levels
        {
            get { return m_levels; }
            set { SetProperty(ref m_levels, value, nameof(Levels)); }
        }

        /// <summary>
        /// 当前选中的关卡索引
        /// </summary>
        public int SelectedLevelIndex
        {
            get { return m_selectedLevelIndex; }
            set { SetProperty(ref m_selectedLevelIndex, value, nameof(SelectedLevelIndex)); }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化模型，加载关卡配置
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 从配置加载关卡列表
        /// </summary>
        /// <param name="levels">关卡数据列表</param>
        public void LoadLevelsFromConfig(List<LevelData> levels)
        {
            m_levels = levels ?? new List<LevelData>();
            NotifyPropertyChanged(nameof(Levels));
        }

        /// <summary>
        /// 获取当前选中的关卡数据
        /// </summary>
        /// <returns>选中的关卡数据，未选中时返回null</returns>
        public LevelData GetSelectedLevel()
        {
            if (m_selectedLevelIndex >= 0 && m_selectedLevelIndex < m_levels.Count)
            {
                return m_levels[m_selectedLevelIndex];
            }
            return null;
        }

        #endregion
    }
}
