namespace MicroKanren
{
    public class Term
    {
        static bool Equal(V v1, V v2) => v1.Index == v2.Index;
        static bool Equal(N n1, N n2) => n1.Value == n2.Value;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equal((dynamic)this, (dynamic)obj);
        }

        public virtual int GetHash() => -1;
        public override int GetHashCode() => GetHash();
    }

    public class V : Term
    {
        public int Index { get; private set; }

        public V(int index)
        {
            Index = index;
        }

        public override int GetHash() => Index.GetHashCode();
    }

    public class N : Term
    {
        public double Value { get; private set; }

        public N(double value)
        {
            Value = value;
        }

        public override int GetHash() => Value.GetHashCode();
    }
}