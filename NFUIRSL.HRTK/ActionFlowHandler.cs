﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFUIRSL.HRTK
{
    public interface IActionFlowHandler
    {
        /// <summary>
        /// 執行完動作後自動選擇下一個動作。
        /// </summary>
        bool AutoNextAction { get; set; }

        /// <summary>
        /// 最後一個執行的動作索引值。<br/>
        /// -1 代表從未執行過動作。
        /// </summary>
        int LastActionIndex { get; }

        /// <summary>
        /// 是否在每一個動作之前顯示確認訊息。
        /// </summary>
        bool ShowMessageBeforeAction { get; set; }

        /// <summary>
        /// 增加動作。增加的順序就是索引、執行的順序。<br/>
        /// 可以使用 Lambda 運算子達成匿名委派，用法示範：<br/>
        /// <c>Add("Example", () => MessageBox.Show("Hi"), "Comment here.");</c>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="comment"></param>
        void Add(string name, Action action, string comment = "--");

        /// <summary>
        /// 清空所有動作。
        /// </summary>
        void Clear();

        /// <summary>
        /// 執行動作。
        /// </summary>
        /// <param name="actionIndex"></param>
        /// <returns>最後一個執行的動作索引值。</returns>
        int Do(int actionIndex);

        /// <summary>
        /// 執行動作。
        /// </summary>
        /// <param name="startActionIndex"></param>
        /// <param name="endActionIndex"></param>
        /// <returns>最後一個執行的動作索引值。</returns>
        int Do(int startActionIndex, int endActionIndex);

        /// <summary>
        /// 執行動作。
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns>最後一個執行的動作索引值。</returns>
        int Do(string actionName);

        /// <summary>
        /// 執行所有動作。
        /// </summary>
        /// <returns>最後一個執行的動作索引值。</returns>
        int DoEach();

        /// <summary>
        /// 執行在 ListView 中選擇的動作。
        /// </summary>
        /// <returns>最後一個執行的動作索引值</returns>
        int DoSelected();

        /// <summary>
        /// 更新清單。
        /// </summary>
        void UpdateListView();
    }

    public struct ActionStruct
    {
        public Action Action;

        public string Comment;

        public string Name;
    }

    public class ActionFlowHandler : IActionFlowHandler
    {
        private readonly List<ActionStruct> Actions = new List<ActionStruct>();
        private readonly ListView ActionsListView;
        private readonly IMessage Message;

        public ActionFlowHandler(ListView actionsListView, IMessage message)
        {
            ActionsListView = actionsListView;
            Message = message;
        }

        public bool AutoNextAction { get; set; } = true;
        public int LastActionIndex { get; private set; } = -1;
        public bool ShowMessageBeforeAction { get; set; } = true;

        public void Add(string name, Action action, string comment = "--")
        {
            // Add to Actions.
            Actions.Add(new ActionStruct()
            {
                Action = action,
                Name = name,
                Comment = comment
            });

            // Update ListView.
            var item = new ListViewItem();
            item.SubItems[0].Text = Convert.ToString(Actions.Count - 1);
            item.SubItems.Add(name);
            item.SubItems.Add(comment);
            ActionsListView.Items.Add(item);
            ActionsListView.Items[0].Selected = true;
        }

        public void Clear()
        {
            Actions.Clear();
        }

        public int Do(int actionIndex)
        {
            var act = Actions[actionIndex];
            if (ShowActionMessageAndContinue(actionIndex))
            {
                act.Action();
                LastActionIndex = actionIndex;
                AutoSelectedNextAction();
            }
            return LastActionIndex;
        }

        public int Do(int startActionIndex, int endActionIndex)
        {
            if (endActionIndex >= startActionIndex)
            {
                for (var i = startActionIndex; i <= endActionIndex; i++)
                {
                    var act = Actions[i];
                    if (ShowActionMessageAndContinue(i))
                    {
                        act.Action();
                        LastActionIndex = i;
                    }
                    else
                    {
                        return LastActionIndex;
                    }
                }
            }
            AutoSelectedNextAction();
            return LastActionIndex;
        }

        public int Do(string actionName)
        {
            // TODO: Use LINQ.
            for (var i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].Name.Equals(actionName))
                {
                    var act = Actions[i];
                    if (ShowActionMessageAndContinue(i))
                    {
                        act.Action();
                        LastActionIndex = i;
                        AutoSelectedNextAction();
                    }
                    else
                    {
                        return LastActionIndex;
                    }
                    break;
                }
            }
            return LastActionIndex;
        }

        public int DoEach()
        {
            for (var i = 0; i < Actions.Count; i++)
            {
                var act = Actions[i];
                if (ShowActionMessageAndContinue(i))
                {
                    act.Action();
                    LastActionIndex = i;
                }
                else
                {
                    return LastActionIndex;
                }
            }
            return LastActionIndex;
        }

        public int DoSelected()
        {
            foreach (int selectedIndex in ActionsListView.SelectedIndices)
            {
                var act = Actions[selectedIndex];
                if (ShowActionMessageAndContinue(selectedIndex))
                {
                    act.Action();
                    LastActionIndex = selectedIndex;
                }
                else
                {
                    return LastActionIndex;
                }
            }
            AutoSelectedNextAction();
            return LastActionIndex;
        }

        public void UpdateListView()
        {
            // Update ListView content.
            ActionsListView.Items.Clear();
            for (var i = 0; i < Actions.Count; i++)
            {
                var item = new ListViewItem();
                item.SubItems[0].Text = i.ToString();
                item.SubItems.Add(Actions[i].Name);
                item.SubItems.Add(Actions[i].Comment);
                ActionsListView.Items.Add(item);
            }

            // Select first item.
            if (ActionsListView.Items.Count > 0)
            {
                ActionsListView.Items[0].Selected = true;
            }

            ResizeListColumnWidth();
        }

        private void AutoSelectedNextAction()
        {
            var selectedCount = ActionsListView.SelectedIndices.Count;
            if (AutoNextAction && selectedCount > 0)
            {
                var lestSelectedIndex = ActionsListView.SelectedIndices[selectedCount - 1];
                if (lestSelectedIndex < (Actions.Count - 1))
                {
                    // Unselect all item.
                    foreach (ListViewItem selectedItem in ActionsListView.SelectedItems)
                    {
                        selectedItem.Selected = false;
                    }

                    // Select next item.
                    ActionsListView.Items[++lestSelectedIndex].Selected = true;
                }
            }
        }

        private void ResizeListColumnWidth()
        {
            // 若要調整資料行中最長專案的寬度，請將 Width 屬性設定為-1。
            // 若要自動調整為數據行標題的寬度，請將 Width 屬性設定為-2。
            foreach (ColumnHeader column in ActionsListView.Columns)
            {
                column.Width = -2;
            }
        }

        /// <summary>
        /// Show action message if enable, and return continue or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>true: Continue; false: Not continue.</returns>
        private bool ShowActionMessageAndContinue(int index)
        {
            if (ShowMessageBeforeAction)
            {
                var text = $"•Index: {index}\r\n" +
                           $"•Name: {Actions[index].Name}\r\n" +
                           $"•Comment: {Actions[index].Comment}";

                var result = Message.Show(text,
                                          "Next Action",
                                          MessageBoxButtons.OKCancel,
                                          MessageBoxIcon.None);

                if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }
    }
}