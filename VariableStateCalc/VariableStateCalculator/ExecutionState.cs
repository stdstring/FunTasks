using System;
using System.Collections.Generic;

namespace VariableStateCalculator
{
    internal class ExecutionState
    {
        public ExecutionState(Int32 assignmentValue) : this(new List<Int32>(), assignmentValue)
        {
        }

        public ExecutionState(IList<Int32> usedParameters, Int32 assignmentValue)
        {
            UsedParameters = usedParameters;
            AssignmentValue = assignmentValue;
        }

        public IList<Int32> UsedParameters { get; }

        public Int32 AssignmentValue { get; }
    }
}
