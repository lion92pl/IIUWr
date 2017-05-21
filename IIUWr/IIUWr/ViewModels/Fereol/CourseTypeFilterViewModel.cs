using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels.Fereol
{
    public class CourseTypeFilterViewModel : INotifyPropertyChanged
    {
        public List<CourseTypeFilterViewModel> Children { get; set; }
        
        public CourseTypeFilterViewModel Parent { get; set; }

        public int Level
        {
            get
            {
                var level = 1;
                var parent = Parent;
                while (parent != null)
                {
                    level++;
                    parent = parent.Parent;
                }
                return level;
            }
        }
        
        public CourseType Type { get; set; }

        private bool _selected;
        public bool Selected
        {
            get
            {
                if (Children != null)
                {
                    return Children.Any(c => c.Selected);
                }
                return _selected;
            }
            set
            {
                if (Children != null)
                {
                    foreach (var child in Children)
                    {
                        child.Selected = value;
                    }
                }
                else if (_selected != value)
                {
                    _selected = value;
                    PropertyChanged.Notify(this);
                    Parent?.OnChildChanged();
                }
            }
        }

        private void OnChildChanged()
        {
            PropertyChanged.Notify(this, nameof(Selected));
            Parent?.OnChildChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
