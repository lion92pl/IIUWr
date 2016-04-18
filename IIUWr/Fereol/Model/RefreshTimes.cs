using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IIUWr.Fereol.Model
{
    public class RefreshTimes : ObservableCollection<RefreshTime>
    {
        public void Set(RefreshTime refreshTime)
        {
            RefreshType type = refreshTime.Type;
            DateTimeOffset time = refreshTime.Time;

            if (type.HasFlag(RefreshType.Failed))
            {
                var item = this.FirstOrDefault(i => refreshTime.Equals(i));
                if (item != null)
                {
                    item.Time = time;
                }
                else
                {
                    Insert(0, refreshTime);
                }
            }
            else if (type.HasFlag(RefreshType.LoggedIn))
            {
                if (type.HasFlag(RefreshType.Full))
                {
                    Remove(RefreshType.LoggedIn | RefreshType.Basic);
                    Remove(RefreshType.Full);
                }
                if (type.HasFlag(RefreshType.Basic))
                {
                    Remove(RefreshType.Basic);
                }

                var item = this.FirstOrDefault(i => refreshTime.Equals(i));
                if (item != null)
                {
                    item.Time = time;
                }
                else
                {
                    int index = Count;
                    if (!type.HasFlag(RefreshType.Full))
                    {
                        index = this.TakeWhile(i => !i.Type.HasFlag(RefreshType.LoggedIn | RefreshType.Full)).Count();
                    }
                    Insert(index, refreshTime);
                }
            }
            else if (type.HasFlag(RefreshType.Full))
            {
                Remove(RefreshType.Basic);
                var item = this.FirstOrDefault(i => refreshTime.Equals(i));
                if (item != null)
                {
                    item.Time = time;
                }
                else
                {
                    int index = this.TakeWhile(i => !i.Type.HasFlag(RefreshType.LoggedIn)).Count();
                    Insert(index, refreshTime);
                }
            }
            else if (type.HasFlag(RefreshType.Basic))
            {
                var item = this.FirstOrDefault(i => refreshTime.Equals(i));
                if (item != null)
                {
                    item.Time = time;
                }
                else
                {
                    int index = this.TakeWhile(i => !i.Type.HasFlag(RefreshType.Full)).Count();
                    Insert(index, refreshTime);
                }
            }
        }

        private void Remove(RefreshType type)
        {
            Remove(new RefreshTime(type));
        }
    }
}
