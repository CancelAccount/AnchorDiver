namespace MyGame.Data
{
    /// <summary>
    /// 关卡会话数据
    /// 跨场景传递当前关卡的配置参数（氧气/锚上限等），由LevelSelectController设置，关卡组件读取
    /// </summary>
    public static class LevelSession
    {
        /// <summary>
        /// 当前关卡数据（进入关卡时设置，退出时清空）
        /// </summary>
        public static LevelData CurrentLevelData { get; private set; }

        /// <summary>
        /// 设置当前关卡数据
        /// </summary>
        public static void SetCurrentLevel(LevelData data)
        {
            CurrentLevelData = data;
        }

        /// <summary>
        /// 获取当前关卡的最大氧气值，无配置时返回默认值
        /// </summary>
        public static float MaxOxygen
        {
            get
            {
                if (CurrentLevelData != null) return CurrentLevelData.maxOxygen;
                return 30f;
            }
        }

        /// <summary>
        /// 获取当前关卡的最大锚数量，无配置时返回默认值
        /// </summary>
        public static int MaxAnchors
        {
            get
            {
                if (CurrentLevelData != null) return CurrentLevelData.maxAnchors;
                return 2;
            }
        }

        /// <summary>
        /// 清空会话数据（退出关卡时）
        /// </summary>
        public static void Clear()
        {
            CurrentLevelData = null;
        }
    }
}
