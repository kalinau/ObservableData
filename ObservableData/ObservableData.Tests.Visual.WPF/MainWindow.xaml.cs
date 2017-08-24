// ReSharper disable All

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ObservableData.Structures;
using ObservableData.Querying;
using ObservableData.Structures.Lists;
using ObservableData.Tests.Core;

namespace ObservableData.Tests.Visual
{
    public struct S
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        private readonly ObservableList<TestEntity> _source = new ObservableList<TestEntity>();

        public MainWindow()
        {
            this.InitializeComponent();

            var sourceSub = _source.ToBindableList(out var bindableSource);
            this.SourceList.ItemsSource = bindableSource;

            _source
                .AsListChangesPlusState()
                .ForSelectConstant(x => x.Value)
                .ToBindableStateProxy(out var state);
            this.AddListView(state);


            var result = new ObservableCollection<int>();
            this.AddListView(result);
            _source
                .AsCollectionChanges()
                .ForSelectConstant(x => x.Value)
                .ForWhereByImmutable(x => x > 5)
                .Subscribe(x => x.ApplyTo(result));
        }

        private void AddListView(IEnumerable source)
        {
            var panel = this.ListsPanel;
            var listView = new ListView()
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                ItemsSource = source
            };
            if (panel.ColumnDefinitions.Count == 0)
            {
                panel.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            }
            listView.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count);
            panel.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            panel.Children.Add(listView);
        }

        private static bool TryGetInt(TextBox textBox, out int value)
        {
            if (int.TryParse(textBox.Text, out value))
            {
                return true;
            }
            return false;
        }

        private TestEntity GetSelected()
        {
            return this.SourceList?.SelectedItem as TestEntity;
        }

        private bool TryGetIndex(out int index)
        {
            return TryGetInt(this.Index, out index);
        }

        private bool TryGetValue(out int value)
        {
            return TryGetInt(this.Value, out value);
        }

        private void CollapseButtons()
        {
            foreach (var button in this.ButtonsPanel.Children.OfType<Button>())
            {
                button.Visibility = Visibility.Collapsed;
            }
        }


        private void ToValueSelectedState(TestEntity entity)
        {
            this.CollapseButtons();
            this.UpdateValueButton.Visibility = Visibility.Visible;
            this.UpdateIndexButton.Visibility = Visibility.Visible;
            this.RemoveButton.Visibility = Visibility.Visible;
            this.ReplaceButton.Visibility = Visibility.Visible;

            this.Index.Text = _source.IndexOf(entity).ToString();
            this.Value.Text = entity.Value.ToString();
        }

        private void ToDefaultState()
        {
            this.CollapseButtons();
            this.AddButton.Visibility = Visibility.Visible;
            this.AddRangeButton.Visibility = Visibility.Visible;
            this.ClearButton.Visibility = Visibility.Visible;
            this.ResetButton.Visibility = Visibility.Visible;

            this.Index.Text = string.Empty;
            this.Value.Text = string.Empty;
            this.SourceList.SelectedItem = null;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (TryGetValue(out var value))
            {
                if (TryGetIndex(out var index))
                {
                    _source.Insert(index, new TestEntity(value));
                }
                else
                {
                    _source.Add(new TestEntity(value));
                }
            }
        }

        private void OnAddRangeClick(object sender, RoutedEventArgs e)
        {
            if (TryGetValue(out var value))
            {
                _source.Add(new[] { new TestEntity(value), new TestEntity(value + 1) });
            }
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems.OfType<TestEntity>().FirstOrDefault();

            if (selected == null)
            {
                this.ToDefaultState();
            }
            else
            {
                this.ToValueSelectedState(selected);
            }
        }

        private void OnUpdateValueClick(object sender, RoutedEventArgs e)
        {
            return;

            if (TryGetValue(out var value))
            {
                this.GetSelected()?.ChangeValue(value);
            }

            this.ToDefaultState();
        }

        private void OnUpdateIndexClick(object sender, RoutedEventArgs e)
        {
            if (TryGetIndex(out var index))
            {
                var originalIndex = _source.IndexOf(this.GetSelected());
                _source.Move(originalIndex, index);
            }

            this.ToDefaultState();
        }

        private void OnReplaceClick(object sender, RoutedEventArgs e)
        {
            if (TryGetInt(Value, out var value))
            {
                var index = _source.IndexOf(this.GetSelected());
                _source[index] = new TestEntity(value);
            }

            this.ToDefaultState();
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            _source.Remove(this.GetSelected());

            this.ToDefaultState();
        }

        private void OnHeaderTap(object sender, MouseButtonEventArgs e)
        {
            this.ToDefaultState();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            _source.Clear();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            if (this.TryGetValue(out var value))
            {
                _source.Reset(new[] { new TestEntity(value), new TestEntity(value + 1) });
            }
        }
    }
}
