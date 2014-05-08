// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.Utilities.Enums
{
    public enum TraceEventId
    {
        General = 0,
        FunctionEntry = 1,
        FunctionExit = 2,
        InboundParameters = 3,
        OutboundParameters = 4,
        Exception = 100,
        Unexcepted = 200,
        Flow = 300
    }
}
