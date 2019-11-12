using System;
using System.Collections.Generic;

namespace VariableStateCalculator
{
    public interface IVariableStateCalculator
    {
        Int32[] Calculate(String source);
    }
}
