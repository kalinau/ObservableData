using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ObservableData.Querying;
using ObservableData.Tests.Core;
using ObservableData.Structures;
using ObservableData.Structures.Lists;

namespace ObservableData.Tests.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        private readonly ObservableList<TestEntity> _source = new ObservableList<TestEntity>();
        private int _index;
        private TestEntity _selected;

        public MainWindow()
        {
            InitializeComponent();
            SourceList.ItemsSource = _source.AsBindableList();
            ResultList.ItemsSource = _source
                .SelectConstant(x => x.Value * 2)
                .WhereImmutable(x => x > 5)
                .AsBindableList();
        }

        private static bool TryGetInt(TextBox textBox, out int value)
        {
            if (int.TryParse(textBox.Text, out value))
            {
                return true;
            }
            return false;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
        }

        private void OnButton(object sender, RoutedEventArgs e)
        {
            if (_selected == null)
            {
                Add();
            }
            else
            {
                Update();
            }
            _selected = null;
            this.Value.Text = string.Empty;
            this.Value.Text = string.Empty;
            this.Button.Content = "Add";
            this.SourceList.SelectedItem = null;
        }

        private void Update()
        {
            if (TryGetInt(Value, out var value))
            {
                _selected?.ChangeValue(value);
            }
        }

        private void Add()
        {
            if (TryGetInt(Value, out var value))
            {
                if (TryGetInt(Index, out var index))
                {
                    _source.Insert(index, new TestEntity(value));
                }
                else
                {
                    _source.Add(new TestEntity(value));
                }
            }
        }

        private void OnSelected(object sender, RoutedEventArgs e)
        {
            _selected = this.SourceList?.SelectedItem as TestEntity;
            if (_selected == null) return;

            this.Value.Text = _selected.Value.ToString();
            this.Index.Text = _source.IndexOf(_selected).ToString();
            this.Button.Content = "Save";
        }
    }
}
