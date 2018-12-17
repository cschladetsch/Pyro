namespace Diver
{
    public class Id
    {
        public int Value => _value;

        public override int GetHashCode()
        {
            return _value;
        }

        private int _value;
    }
}