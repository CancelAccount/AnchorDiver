using UnityEngine;
using Inventory.data;
using Inventory.view;
using MyGame.Events;
using MyGame.UI;

namespace Inventory.controller
{
    /// <summary>
    /// 背包控制器，负责处理背包的逻辑和数据管理
    /// 继承BaseController以遵循MVC规范
    /// </summary>
    public class InventoryController : BaseController<InventoryView, InventoryModel>
    {
        #region 字段

        [Tooltip("物品数据库")]
        [SerializeField] private ItemDatabase itemDatabase;

        private GameControl _inputActions;

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化控制器
        /// </summary>
        private void Awake()
        {
            _inputActions = new GameControl();
            InitializeMVCComponents();
            Initialize();
        }

        /// <summary>
        /// 启用控制器
        /// </summary>
        private void OnEnable()
        {
            _inputActions.Enable();
        }

        /// <summary>
        /// 禁用控制器
        /// </summary>
        private void OnDisable()
        {
            _inputActions.Disable();
        }

        /// <summary>
        /// 销毁时清理
        /// </summary>
        private void OnDestroy()
        {
            if (m_model != null)
            {
                m_model.OnInventoryChanged -= UpdateInventoryView;
                m_model.Cleanup();
            }
        }

        /// <summary>
        /// 初始化MVC组件
        /// </summary>
        private void InitializeMVCComponents()
        {
            // 创建并初始化Model
            CreateAndInitializeModel();

            // 订阅Model变更事件
            m_model.OnInventoryChanged += UpdateInventoryView;

            // 查找或获取View引用
            if (m_view == null)
            {
                m_view = GetComponent<InventoryView>();
                if (m_view == null)
                {
                    m_view = GetComponentInChildren<InventoryView>(true);
                }
            }

            if (m_view != null)
            {
                m_view.InitializeInventory(m_model.Capacity);
            }
            else
            {
                Debug.LogError("InventoryController: 视图未找到");
            }

            // 测试添加物品
            AddTestItems();
        }

        /// <summary>
        /// 输入检测更新
        /// </summary>
        private void Update()
        {
            if (_inputActions.GamePlay.Inventory.triggered)
            {
                GameEvents.TriggerMenuShow(UIType.Inventory, true);
            }
        }

        #endregion

        #region 测试数据

        /// <summary>
        /// 添加测试物品
        /// </summary>
        private void AddTestItems()
        {
            AddItem("health_potion", 5);
            AddItem("sword", 1);
            AddItem("gold_coin", 42);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加物品
        /// </summary>
        public bool AddItem(string itemID, int quantity = 1)
        {
            ItemData item = itemDatabase.GetItem(itemID);
            if (item == null) return false;

            return m_model.AddItem(item, quantity);
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public bool RemoveItem(string itemID, int quantity = 1)
        {
            return m_model.RemoveItem(itemID, quantity);
        }

        /// <summary>
        /// 移动物品
        /// </summary>
        public void MoveItem(int fromIndex, int toIndex)
        {
            m_model.MoveItem(fromIndex, toIndex);
        }

        /// <summary>
        /// 使用物品（按槽位索引）
        /// </summary>
        public void UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= m_model.Items.Count) return;

            var item = itemDatabase.GetItem(m_model.Items[slotIndex].ItemID);
            if (item != null && item.Type == ItemData.ItemType.Consumable)
            {
                Debug.Log($"使用物品: {item.Name}");
                RemoveItem(item.ID, 1);
            }
        }

        /// <summary>
        /// 使用物品（按物品数据）
        /// </summary>
        internal void UseItem(ItemData currentItem)
        {
            if (currentItem != null && currentItem.Type == ItemData.ItemType.Consumable)
            {
                Debug.Log($"使用物品: {currentItem.Name}");
                RemoveItem(currentItem.ID, 1);
            }
        }

        /// <summary>
        /// 显示物品详情
        /// </summary>
        public void ShowItemDetails(ItemData item)
        {
            Debug.Log($"显示物品详情: {item.Name}\n{item.Description}");
        }

        #endregion

        #region View更新

        /// <summary>
        /// 更新背包视图
        /// </summary>
        private void UpdateInventoryView()
        {
            if (m_view == null) return;

            for (int i = 0; i < m_model.Capacity; i++)
            {
                if (i < m_model.Items.Count)
                {
                    var item = itemDatabase.GetItem(m_model.Items[i].ItemID);
                    m_view.UpdateSlot(i, item, m_model.Items[i].Quantity);
                }
                else
                {
                    m_view.UpdateSlot(i, null, 0);
                }
            }

            m_view.UpdateCapacity(m_model.Items.Count, m_model.Capacity);
        }

        #endregion
    }
}
