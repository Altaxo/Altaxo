﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Altaxo;
using System;

namespace Altaxo.AddInItems
{
  public interface IOwnerState
  {
    System.Enum InternalState
    {
      get;
    }
  }

  /// <summary>
  /// Condition evaluator that compares the state of the parameter with a specified value.
  /// The parameter has to implement <see cref="IOwnerState"/>.
  /// </summary>
  public class OwnerStateConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object parameter, Condition condition)
    {
      if (parameter is IOwnerState)
      {
        try
        {
          System.Enum state = ((IOwnerState)parameter).InternalState;
          System.Enum conditionEnum = (System.Enum)Enum.Parse(state.GetType(), condition.Properties["ownerstate"]);

          int stateInt = Int32.Parse(state.ToString("D"));
          int conditionInt = Int32.Parse(conditionEnum.ToString("D"));

          return (stateInt & conditionInt) > 0;
        }
        catch (Exception ex)
        {
          throw new BaseException("can't parse '" + condition.Properties["state"] + "'. Not a valid value.", ex);
        }
      }
      return false;
    }
  }
}
