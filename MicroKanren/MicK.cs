
namespace MicroKanren;

using Stream = IEnumerable<State>;
using Goal = Func<State, IEnumerable<State>>;//Note: Goal returns a Stream but making this explicit messes with C#'s type system.

public class MicK
{
    public static State EmptyState() => new();

    public static Goal Unification(Term u, Term v) =>
        (State s_c) =>
        {
            var s = Unify(u, v, s_c.Substitution);
            var result = new List<State>();
            return s == null ? result : result.Append(new State(s, s_c.VariableCounter));
        };

    static Substitution? Unify(Term u, Term v, Substitution s)
    {
        u = Walk(u, s);
        v = Walk(v, s);

        if (IsVar(u) && IsVar(v) && u == v) return s;
        else if (IsVar(u)) return ExtendSubstitution(u, v, s);
        else if (IsVar(v)) return ExtendSubstitution(v, u, s);
        else if (u == v) return s;

        return null;
    }

    static bool IsVar(Term term) =>
        term.GetType() == typeof(V);

    static Substitution ExtendSubstitution(Term variable, Term value, Substitution substitution) =>
        substitution.Add(variable, value);

    static Term Walk(Term u, Substitution s)
    {
        var pr = IsVar(u) && s.HasValue(u);
        return pr ? Walk(s.GetValueOf(u), s) : u;
    }

    public static Goal Fresh(Func<Term, Goal> f) =>
        (State s_c) =>
        {
            var c = s_c.VariableCounter;
            return f(new V(c))(new State(s_c.Substitution, c + 1));
        };

    public static Goal Conjunction(Goal g1, Goal g2) =>
        (State s_c) => Bind(g1(s_c), g2);

    static Stream Bind(Stream s, Goal g)
    {
        if (s == null || !s.Any()) return new List<State>();
        return MPlus(g(s.First()), Bind(s.Skip(1), g));
    }

    public static Goal Disjunction(Goal g1, Goal g2) =>
        (State s_c) => MPlus(g1(s_c.Clone()), g2(s_c.Clone()));

    //static Stream MPlus(Stream stream1, Stream stream2) =>
    //    Interleave(stream1, stream2);

    static Stream MPlus(Stream stream1, Stream stream2) =>
        stream1.Concat(stream2);

    public static IEnumerable<T> Interleave<T>(IEnumerable<T> s1, IEnumerable<T> s2)
    {
        var s1e = s1.GetEnumerator();
        var s2e = s2.GetEnumerator();
        bool s1e_isDone;
        while (true)
        {
            if (!s1e.MoveNext())
            {
                s1e_isDone = true;
                break;
            }

            yield return s1e.Current;
            if (!s2e.MoveNext())
            {
                s1e_isDone = false;
                break;
            }

            yield return s2e.Current;
        }

        var se = s1e_isDone ? s2e : s1e;

        while (true)
        {
            if (!se.MoveNext()) break;
            yield return se.Current;
        }
    }
}