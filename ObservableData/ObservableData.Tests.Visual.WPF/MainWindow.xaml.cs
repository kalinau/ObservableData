// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ObservableData.Structures;
using ObservableData.Querying;
using ObservableData.Structures.Lists;
using ObservableData.Structures.Utils;
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
        private int _index;
        private TestEntity _selected;

        public MainWindow()
        {
            this.InitializeComponent();

            var s = new S() {Value = 5};
            var s2 = new S() {Value = 1};

            Debug.WriteLine(s.GetHashCode());
            Debug.WriteLine(s2.GetHashCode());

            //var observableList = new ObservableCollection<int>();
            //this.ResultList.ItemsSource = observableList;

            var sub = _source
                .AsChangedListDataObservable()
                .ForSelectConstant(x => x.Value)
                .ToBindableStateProxy(out var state);
            this.ResultList.ItemsSource = state;
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
