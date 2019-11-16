using System;
using System.Collections.Generic;
using System.Linq;

namespace VariableStateCalculator
{
    internal sealed class ExecutionFrame
    {
        public ExecutionFrame()
        {
            States = new List<ExecutionState>();
            UsedParameter = null;
        }

        public ExecutionFrame(Int32 usedParameter)
        {
            States = new List<ExecutionState>();
            UsedParameter = usedParameter;
        }

        public Int32? UsedParameter { get; }

        public IList<ExecutionState> States { get; }

        public void Merge(ExecutionFrame childFrame)
        {
            if (!childFrame.UsedParameter.HasValue)
                throw new InvalidOperationException();
            Int32 parameter = childFrame.UsedParameter.Value;
            foreach (ExecutionState childState in childFrame.States)
            {
                List<Int32> usedParameters = new List<Int32>(childState.UsedParameters);
                usedParameters.Add(parameter);
                usedParameters.Sort();
                ExecutionState newState = new ExecutionState(usedParameters, childState.AssignmentValue);
                ExecutionState existingState = States.FirstOrDefault(state => IsPrefix(usedParameters, state.UsedParameters));
                if (existingState != null)
                    States.Remove(existingState);
                States.Add(newState);
            }
        }

        private Boolean IsPrefix(IList<Int32> left, IList<Int32> right)
        {
            if (right.Count < left.Count)
                return false;
            for (Int32 index = 0; index < left.Count; ++index)
            {
                if (left[index] != right[index])
                    return false;
            }
            return true;
        }
    }
}
