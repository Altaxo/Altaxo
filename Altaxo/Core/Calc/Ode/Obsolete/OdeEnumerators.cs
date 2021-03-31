#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Specifies the type of the relative error and absolute error tolerances.
  /// </summary>
  public enum ErrorToleranceEnum
  {
    /// <summary>
    /// The relative error and absolute error tolerances are scalars. The program keeps the error of Y(I) below RelTol*Abs(Y[i]) + AbsTol.
    /// </summary>
    Scalar,

    /// <summary>
    /// The relative error and absolute error tolerances are arrays. In this case the program keeps the  error of Y(I) below RelTolArray[I]*Abs(Y(I))+AbsTolArray[i].
    /// </summary>
    Array
  }
}
