using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Data
{
    /// <summary>
    /// 关卡列表配置（ScriptableObject）
    /// 集中管理所有关卡数据，一个文件即可配置全部关卡
    /// </summary>
    [CreateAssetMenu(fileName = "LevelListConfig", menuName = "GameData/LevelListConfig")]
    public class LevelListConfig : ScriptableObject
    {
        [Header("关卡列表")]
        [Tooltip("按顺序排列的关卡数据")]
        public List<LevelData> levels = new();
    }
}
