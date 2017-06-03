namespace IIUWr.Fereol.Model
{
    public class AuthenticationStatus
    {
        public bool Authenticated { get; set; }

        public bool IsStudent { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Authenticated ? Name + ", " + (IsStudent ? "" : "not ") + "student" : "Unauthenticated";
        }

        public override bool Equals(object obj)
        {
            if (obj is AuthenticationStatus otherStatus)
            {
                return Authenticated == otherStatus.Authenticated
                    && IsStudent == otherStatus.IsStudent
                    && Name == otherStatus.Name;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Authenticated ? 1 << 31 : 0)
                 | (IsStudent ? 1 << 30 : 0)
                 | Name.GetHashCode() >> 2;
        }

        public static bool operator==(AuthenticationStatus a, AuthenticationStatus b)
        {
            if (ReferenceEquals(a,b))
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            return a.Authenticated == b.Authenticated
                && a.IsStudent == b.IsStudent
                && a.Name == b.Name;
        }

        public static bool operator !=(AuthenticationStatus a, AuthenticationStatus b)
        {
            return !(a == b);
        }
    }
}
