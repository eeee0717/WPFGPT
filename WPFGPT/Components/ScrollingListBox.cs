﻿using System.Windows.Controls;

namespace WPFGPT.Components;

public class ScrollingListBox: ListBox
{
    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems == null) return;
        var newItemCount = e.NewItems.Count;

        if (newItemCount > 0)
            this.ScrollIntoView(e.NewItems[newItemCount - 1]!);

        base.OnItemsChanged(e);
    }
}