using IIUWr.Fereol.Model.Enums;

namespace IIUWr.Fereol.Model
{
    public class Semester
    {
        //TODO implement
        public string Year { get; set; }

        public YearHalf YearHalf { get; set; }

        public int Id { get; set; }

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return Id.Equals((obj as Semester).Id);
        }
        
        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Semester x, Semester y)
        {
            return x?.Id == y?.Id;
        }

        public static bool operator !=(Semester x, Semester y)
        {
            return x?.Id != y?.Id;
        }

        #endregion

        public override string ToString()
        {
            return $"{Year} {YearHalf}";
        }
    }
}
