namespace MicroKanren
{
    public class Substitution
    {
        readonly Dictionary<Term, Term> _substitution;

        public Substitution() =>
            _substitution = new Dictionary<Term, Term>();

        public Substitution(Dictionary<Term, Term> substitution) =>
            _substitution = substitution;

        public Substitution Add(Term variable, Term value)
        {
            _substitution.Add(variable, value);
            return this;
        }

        public bool HasValue(Term variable) =>
            _substitution.ContainsKey(variable);

        public Term GetValueOf(Term t) =>
            _substitution[t];

        public Substitution Clone()
        {
            var newSubstitution = new Dictionary<Term, Term>();
            foreach (var pair in _substitution)
                newSubstitution[pair.Key] = pair.Value;
            return new Substitution(newSubstitution);
        }
    }
}