using MicroKanren;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MicroKanrenTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var goal = MicK.Fresh((q) => MicK.Unification(q, new N(5)));
        var result = goal(MicK.EmptyState());

        Assert.AreEqual(1, result.Count());
        State actual = result.First();
        Assert.AreEqual(1, actual.VariableCounter);
        Assert.IsTrue(actual.Substitution.HasValue(new V(0)));
        Assert.AreEqual<Term>(new N(5), actual.Substitution.GetValueOf(new V(0)));
    }

    [TestMethod]
    public void TestMethod2()
    {
        var goal = MicK.Conjunction(
            MicK.Fresh((a) => MicK.Unification(a, new N(7))),
            MicK.Fresh((b) => MicK.Disjunction(MicK.Unification(b, new N(5)), MicK.Unification(b, new N(6)))));
        var result = goal(MicK.EmptyState());

        Assert.AreEqual(2, result.Count());

        State first = result.First();
        Assert.AreEqual(2, first.VariableCounter);
        Assert.IsTrue(first.Substitution.HasValue(new V(0)));
        Assert.IsTrue(first.Substitution.HasValue(new V(1)));
        Assert.AreEqual<Term>(new N(7), first.Substitution.GetValueOf(new V(0)));
        Assert.AreEqual<Term>(new N(5), first.Substitution.GetValueOf(new V(1)));

        State second = result.Last();
        Assert.AreEqual(2, second.VariableCounter);
        Assert.IsTrue(second.Substitution.HasValue(new V(0)));
        Assert.IsTrue(second.Substitution.HasValue(new V(1)));
        Assert.AreEqual<Term>(new N(7), second.Substitution.GetValueOf(new V(0)));
        Assert.AreEqual<Term>(new N(6), second.Substitution.GetValueOf(new V(1)));
    }

    [TestMethod]
    public void TestMethod3()
    {
        var goal = MicK.Conjunction(
            MicK.Fresh((a) => MicK.Unification(a, new N(7))),
            MicK.Fresh((b) => MicK.Unification(b, new N(5))));
        var result = goal(MicK.EmptyState());

        Assert.AreEqual(1, result.Count());
        State actual = result.First();
        Assert.AreEqual(2, actual.VariableCounter);
        Assert.IsTrue(actual.Substitution.HasValue(new V(0)));
        Assert.IsTrue(actual.Substitution.HasValue(new V(1)));
        Assert.AreEqual<Term>(new N(7), actual.Substitution.GetValueOf(new V(0)));
        Assert.AreEqual<Term>(new N(5), actual.Substitution.GetValueOf(new V(1)));
    }

    [TestMethod]
    public void TestMethod4()
    {
        var goal = MicK.Disjunction(
            MicK.Fresh((a) => MicK.Unification(a, new N(7))),
            MicK.Fresh((b) => MicK.Unification(b, new N(5))));
        var result = goal(MicK.EmptyState());

        Assert.AreEqual(2, result.Count());

        State first = result.First();
        Assert.AreEqual(1, first.VariableCounter);
        Assert.IsTrue(first.Substitution.HasValue(new V(0)));
        Assert.AreEqual<Term>(new N(7), first.Substitution.GetValueOf(new V(0)));

        State second = result.Last();
        Assert.AreEqual(1, second.VariableCounter);
        Assert.IsTrue(second.Substitution.HasValue(new V(0)));
        Assert.AreEqual<Term>(new N(5), second.Substitution.GetValueOf(new V(0)));
    }

    [TestMethod]
    public void TestInfiniteInterleave()
    {
        var digits = MicK.Interleave(infinite(1), infinite(2)).Take(42);

        Assert.AreEqual(42, digits.Count());
        Assert.AreEqual(1, digits.First());
        Assert.AreEqual(2, digits.ElementAt(1));

        static IEnumerable<object> infinite(int n)
        {
            while (true)
                yield return n;
        }
    }

    [TestMethod]
    public void InterleavesFirstArgCanBeShorterThanItsLast()
    {
        var digits = MicK.Interleave(A(1, 2), A(4, 5, 6, 7));

        Assert.AreEqual(6, digits.Count());
        CollectionAssert.AreEqual(A(1, 4, 2, 5, 6, 7), digits.ToArray());
    }

    [TestMethod]
    public void InterleavesFirstArgCanBeLongerThanItsLast()
    {
        var digits = MicK.Interleave(A(1, 2, 3, 4), A(8, 9));

        Assert.AreEqual(6, digits.Count());
        CollectionAssert.AreEqual(A(1, 8, 2, 9, 3, 4), digits.ToArray());
    }

    static int[] A(params int[] args) => args;

    //[TestMethod]
    //public void InfiniteGoalsWork()
    //{
    //    //Note: fives :: Term -> Goal.
    //    static Func<State, IEnumerable<State>> fives(Term x)
    //    {
    //        Func<State, IEnumerable<State>> g1 = MicK.Unification(x, new N(5));
    //        Func<State, IEnumerable<State>> g2 = (s_c) => fives(x)(s_c);
    //        //Func<State, IEnumerable<State>> g2 = fives(x);//Oops: Infinite recursion
    //        Func<State, IEnumerable<State>> func = MicK.Disjunction(g1, g2);
    //        return func;
    //    }
    //    //Note: Fresh :: (Term -> Goal) -> Goal so f :: Goal.
    //    var f = MicK.Fresh(fives);
    //    State arg = MicK.EmptyState();
    //    var result = f(arg).Take(5);

    //    Assert.AreEqual(2, result.Count());

    //    State first = result.First();
    //    Assert.AreEqual(1, first.VariableCounter);
    //    Assert.IsTrue(first.Substitution.HasValue(new V(0)));
    //    Assert.AreEqual<Term>(new N(7), first.Substitution.GetValueOf(new V(0)));

    //    State second = result.Last();
    //    Assert.AreEqual(1, second.VariableCounter);
    //    Assert.IsTrue(second.Substitution.HasValue(new V(0)));
    //    Assert.AreEqual<Term>(new N(5), second.Substitution.GetValueOf(new V(0)));
    //}
}