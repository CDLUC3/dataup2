using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities.Enums
{
    /// <summary>
    /// Generic error type enum values.
    /// </summary>
    public enum ErrorType
    {
        Names = 0,
        Worksheet = 1,
        Charts = 2,
        Comments = 3,
        Shapes = 4,
        MergedCell = 5,
        Tables = 6,
        BlankCell = 7,
        Commas = 8,
        SpecialCharacter = 9,
        ColorCoded = 10,
        MixedType = 11,
        Noncontiguous = 12,
        Pictures = 13
    }
}
