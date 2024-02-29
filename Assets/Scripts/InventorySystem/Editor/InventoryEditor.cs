using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace InventorySystem.Editor
{
    [CustomEditor(typeof(BasicInventory))]
    public class InventoryEditor : UnityEditor.Editor
    {
        public VisualTreeAsset visualTree;
        public VisualTreeAsset listElement;

        private BasicInventory m_Inventory;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            m_Inventory = (BasicInventory)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customInspector = new();
            visualTree.CloneTree(customInspector);


            // Create some list of data, here simply numbers in interval [1, 1000]
            //const int itemCount = 1000;
            //var items = new List<string>(itemCount);
            //for (int i = 1; i <= itemCount; i++)
            //    items.Add(i.ToString());
            var items = m_Inventory.GetItemList();

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () =>
            {
                VisualElement visualElement = new();
                listElement.CloneTree(visualElement);
                return visualElement;
            };

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var item = items[i];
                e.Q<Label>("ItemName").text = item.Key.ItemName;
                e.Q<Label>("ItemNumber").text = item.Value.ToString();
                e.Q<Label>("ItemBuyPrice").text = item.Key.BuyPrice.ToString();
            };

            var listView = customInspector.Q<ListView>();
            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.itemsSource = items;
            listView.selectionType = SelectionType.None;


            // Callback invoked when the user double clicks an item
            //listView.onItemsChosen += Debug.Log;

            // Callback invoked when the user changes the selection inside the ListView
            //listView.onSelectionChange += Debug.Log;


            return customInspector;
        }
    }
}
