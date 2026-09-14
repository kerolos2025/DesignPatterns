namespace _1_Singleton
{
    public class RequestCounter : IRequestCounter
    {
        private int _count;

        public int GetCount()
        {
            return _count;
        }

        public void Increment()
        {
            _count++;
        }
    }
}
