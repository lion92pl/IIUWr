using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Utils.Refresh
{
    public class RefreshTimesManager
    {
        private Dictionary<object, RefreshTimes> _refreshTimes = new Dictionary<object, RefreshTimes>();

        // remember to lock operations on collection
        public RefreshTimes this[object obj]
        {
            get
            {
                RefreshTimes result;
                if (!_refreshTimes.TryGetValue(obj, out result))
                {
                    result = new RefreshTimes();
                    _refreshTimes[obj] = result;
                }
                return result;
            }
        }

        public void Set(object obj, RefreshTime refreshTime)
        {
            this[obj].Set(refreshTime);
        }

        public void Set(object obj, RefreshType refreshType)
        {
            Set(obj, new RefreshTime(refreshType));
        }
    }
}
