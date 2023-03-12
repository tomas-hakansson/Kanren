using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroKanren
{
    public class State
    {
        public Substitution Substitution { get; private set; }
        public int VariableCounter { get; private set; }

        public State()
        {
            Substitution = new Substitution();
            VariableCounter = 0;
        }

        public State(Substitution substitution, int variableCounter)
        {
            Substitution = substitution;
            VariableCounter = variableCounter;
        }

        public State Clone() =>
            new(Substitution.Clone(), VariableCounter);
    }
}
