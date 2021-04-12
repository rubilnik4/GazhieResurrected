﻿using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace GadzhiModules.Helpers.Wpf.Behaviors
{
    public class DataGridSelectedItemsBlendBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;
        }

        private void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var array = new object[AssociatedObject.SelectedItems.Count];
            AssociatedObject.SelectedItems.CopyTo(array, 0);
            SelectedItems = array;
        }

        private static readonly DependencyProperty _selectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(DataGridSelectedItemsBlendBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IList SelectedItems
        {
            get => (IList)GetValue(_selectedItemsProperty);
            set => SetValue(_selectedItemsProperty, value);
        }

    }
}
