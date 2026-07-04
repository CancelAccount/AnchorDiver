using UnityEngine;

namespace MyGame.Data
{
    /// <summary>
    /// 关卡进度管理
    /// 使用PlayerPrefs持久化关卡解锁状态
    /// </summary>
    public static class LevelProgress
    {
        private const string UNLOCK_KEY_PREFIX = "LevelUnlocked_";
        private const string CURRENT_LEVEL_KEY = "CurrentLevelId";

        /// <summary>
        /// 记录当前正在游玩的关卡ID（进入关卡时设置）
        /// </summary>
        public static int CurrentLevelId
        {
            get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);
            set => PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value);
        }

        /// <summary>
        /// 解锁指定关卡
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        public static void UnlockLevel(int levelId)
        {
            PlayerPrefs.SetInt(UNLOCK_KEY_PREFIX + levelId, 1);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 检查关卡是否已解锁
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <param name="isUnlockedByDefault">配置中的默认解锁状态</param>
        /// <returns>true=已解锁</returns>
        public static bool IsLevelUnlocked(int levelId, bool isUnlockedByDefault)
        {
            if (isUnlockedByDefault) return true;
            return PlayerPrefs.GetInt(UNLOCK_KEY_PREFIX + levelId, 0) == 1;
        }
    }
}
