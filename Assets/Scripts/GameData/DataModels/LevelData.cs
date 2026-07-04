using System;
using MyGame.Control;
using UnityEngine;

namespace MyGame.Data
{
    /// <summary>
    /// 单个关卡的数据定义（普通可序列化类）
    /// 用于在编辑器中配置关卡的展示信息和对应的场景名称
    /// </summary>
    [Serializable]
    public class LevelData
    {
        [Header("关卡基本信息")]
        [Tooltip("关卡唯一ID")]
        public int levelId;

        [Tooltip("关卡显示名称")]
        public string levelName;

        [Tooltip("关卡对应的场景名称")]
        public string sceneName;

        [Tooltip("关卡描述（可选）")]
        [TextArea(2, 4)]
        public string description;

        [Tooltip("是否默认解锁")]
        public bool isUnlockedByDefault = false;

        [Header("关卡参数")]
        [Tooltip("最大氧气值（秒）")]
        public float maxOxygen = 30f;

        [Tooltip("最大锚数量")]
        public int maxAnchors = 2;

        /// <summary>
        /// 运行时是否已解锁（检查PlayerPrefs + 默认解锁状态）
        /// </summary>
        public bool IsUnlocked
        {
            get { return LevelProgress.IsLevelUnlocked(levelId, isUnlockedByDefault); }
        }
    }
}
