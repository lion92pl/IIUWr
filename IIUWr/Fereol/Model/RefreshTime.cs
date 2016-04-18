using IIUWr.Fereol.Model.Enums;
using System;
using System.ComponentModel;

namespace IIUWr.Fereol.Model
{
    public class RefreshTime : INotifyPropertyChanged
    {
        public RefreshTime(RefreshType type) : this(type, DateTimeOffset.Now) { }

        public RefreshTime(RefreshType type, DateTimeOffset time)
        {
            Type = type;
            _time = time;
        }

        public static implicit operator RefreshTime(RefreshType type)
        {
            return new RefreshTime(type);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RefreshType Type { get; }

        private DateTimeOffset _time;
        public DateTimeOffset Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        public bool IsSuccess
        {
            get
            {
                return !Type.HasFlag(RefreshType.Failed);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RefreshTime)
            {
                return Type == ((RefreshTime)obj).Type;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)Type;
        }
    }
}
