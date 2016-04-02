using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Model
{
    public class CourseType
    {
        private CourseType() { }

        /// <summary>
        /// Used to retrieve translated names of type
        /// </summary>
        public string NameKey { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Id}: {NameKey}";
        }

        public List<CourseType> Children { get; set; }

        public CourseType Parent
        {
            get
            {
                var path = Path(Id);
                if (path.Count > 1)
                {
                    return path[path.Count - 2];
                }
                return null;
            }
        }
        
        public static CourseType Find(int id)
        {
            var path = Path(id);
            return path.LastOrDefault();
        }

        public static List<CourseType> Path(int id)
        {
            List<CourseType> path = new List<CourseType>();

            Path(id, path, Types);

            return path;
        }

        private static bool Path(int id, List<CourseType> path, List<CourseType> currentLevel)
        {
            if (currentLevel == null)
            {
                return false;
            }

            CourseType found = currentLevel.FirstOrDefault(t => t.Id == id);
            if (found != null)
            {
                path.Insert(0, found);
                return true;
            }

            foreach (CourseType type in currentLevel)
            {
                if (Path(id, path, type.Children))
                {
                    path.Insert(0, type);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hierarchy of types
        /// </summary>
        public static List<CourseType> Types = new List<CourseType>
        {
            new CourseType { NameKey = "Inf", Id = 1, Children = new List<CourseType>
            {
                new CourseType { NameKey = "I1", Id = 5 },
                new CourseType { NameKey = "I2", Id = 6, Children = new List<CourseType>
                {
                    new CourseType { NameKey = "I2T", Id = 38 },
                    new CourseType { NameKey = "I2Z", Id = 39 }
                } },
                new CourseType { NameKey = "Iinż", Id = 7 }
            } },
            new CourseType { NameKey = "O", Id = 2, Children = new List<CourseType>
            {
                new CourseType { NameKey = "O1", Id = 8 },
                new CourseType { NameKey = "O2", Id = 9 },
                new CourseType { NameKey = "O3", Id = 10 },
                new CourseType { NameKey = "Oinż", Id = 11 }
            } },
            new CourseType { NameKey = "KursySeminaria", Id = 3, Children = new List<CourseType>
            {
                new CourseType { NameKey = "Kurs", Id = 12, Children = new List<CourseType>
                {
                    new CourseType { NameKey = "K1", Id = 36 },
                    new CourseType { NameKey = "K2", Id = 37 },
                    new CourseType { NameKey = "Kinż", Id = 40 }
                } },
                new CourseType { NameKey = "Projekt", Id = 13 },
                new CourseType { NameKey = "Seminarium", Id = 14 }
            } },
            new CourseType { NameKey = "Nieinformatyczny", Id = 15 },
            new CourseType { NameKey = "WF", Id = 16 },
            new CourseType { NameKey = "Lektorat", Id = 17 },
            new CourseType { NameKey = "Inne", Id = 35 }
        };
    }
}
