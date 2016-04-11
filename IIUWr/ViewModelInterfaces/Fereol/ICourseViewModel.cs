using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ICourseViewModel : IRefreshable, INotifyPropertyChanged
    {
        Course Course { get; set; }
    }
}
