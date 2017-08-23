using System.ComponentModel;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace ObservableData.Tests.Core
{
    public class TestEntity : INotifyPropertyChanged
    {
        private int _value;
        [NotNull] private readonly Subject<int> _subject = new Subject<int>();

        public TestEntity(int value)
        {
            _value = value;
        }

        public int Value => _value;

        public void ChangeValue(int value)
        {
            _value = value;
            this.OnPropertyChanged(nameof(this.Value));
            _subject.OnNext(_value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"TestEntity: {_value}";
    }
}