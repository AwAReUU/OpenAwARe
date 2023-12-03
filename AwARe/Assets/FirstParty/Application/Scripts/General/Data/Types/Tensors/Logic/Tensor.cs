namespace AwARe.Data.Logic
{
    namespace Interfaces
    {
        public interface ITensor1<T>
        {
            public T x { get; set; }
            public T this[int idx] { get; set; }
            public T[] ToArray();
        }
        public interface ITensor2<T> : ITensor1<T>
        {
            public T y { get; set; }
        }

        public interface ITensor3<T> : ITensor2<T>
        {
            public T z { get; set; }
        }

        public interface ITensor4<T> : ITensor3<T>
        {
            public T w { get; set; }
        }
    }
}