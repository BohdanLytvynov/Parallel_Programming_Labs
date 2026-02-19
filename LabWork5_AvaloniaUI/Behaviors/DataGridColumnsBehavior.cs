using Avalonia.Controls;
using Avalonia.Data;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using System;

namespace LabWork5_AvaloniaUI.Behaviors
{
    /// <summary>
    /// Behavior for DataGrid Called when we change the Amount of Columns
    /// </summary>
    public class DataGridColumnsBehavior : Behavior<DataGrid>
    {
        //Property for Binding. Same as the DependencyProperty
        public static readonly StyledProperty<int> ColumnCountProperty =
            AvaloniaProperty.Register<DataGridColumnsBehavior, int>(nameof(ColumnCount));
        //Accessor and Mutator
        public int ColumnCount
        {
            get => GetValue(ColumnCountProperty);
            set => SetValue(ColumnCountProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            //Subscribe to the method that will be called when Styled Property will change
            this.GetObservable(ColumnCountProperty).Subscribe(o => UpdateColumns());
        }

        private void UpdateColumns()
        {
            if (AssociatedObject == null) return;//If Datagrid is not initialized - return

            var currentItems = AssociatedObject.ItemsSource;//Temp storage for DataGrid
            AssociatedObject.ItemsSource = null;//Clear the Binding to actual datasource.
                                                //This is done due to sync data later

            AssociatedObject.Columns.Clear();//Clear DataGrid
            for (int i = 0; i < ColumnCount; i++)
            {
                //Building the DataGrid Template
                AssociatedObject.Columns.Add(new DataGridTextColumn
                {
                    Header = $"C{i}",
                    Binding = new Binding($"[{i}]"),
                    Width = new DataGridLength(90)
                });
            }
            //Fill data grid with new values
            AssociatedObject.ItemsSource = currentItems;
        }
    }
}
