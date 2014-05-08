// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Research.DataOnboarding.Utilities.Enums;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using System.Text;
    using System.Net;

    /// <summary>
    /// Defines helper methods to trace call flow information. 
    /// </summary>
    /// <remarks>
    /// I) Future Enhancements: This is a vanilla implementation and need to be
    /// refactored to do the following:
    /// 1. Read the source names from configuration and cache it. 
    /// </remarks>
    /// Vanilla tracing implementation 
    [ExcludeFromCodeCoverage] 
    public sealed class DiagnosticsProvider
    {
        private const string RootSourceName = "Microsoft.Research.DataOnboarding";
        private TraceSource source;

        public DiagnosticsProvider(Type type)
        {
            if (null == type || string.IsNullOrEmpty(type.Namespace))
            {
                InitializeDefaultSource();
                return;
            }
            
            // Check if the level is configured. If not use the default source
            // Else, initialize trace source.
            // Note: This needs to be changed to check for existence of trace source
            // configuration instead of the level configuration. 
            string level = ConfigReader<string>.GetSetting(type.Namespace, string.Empty);
            if (level.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                InitializeDefaultSource();
            }
            else
            {
                source = new TraceSource(type.Namespace);
                source.Switch = new SourceSwitch(type.Namespace);
                source.Switch.Level = SourceLevelFromString(level);
            }
        }

        #region Trace Helper Methods

        public void WriteTrace(TraceEventType evtType, TraceEventId eventId, string msg)
        {
            if (source == null)
            {
                return;
            }
            source.TraceEvent(evtType, (int)eventId, msg);
        }

        public void WriteTrace(TraceEventType evtType, TraceEventId eventId, string format, params object[] args)
        {
            if (source == null)
            {
                return;
            }
            source.TraceEvent(evtType, (int)eventId, format, args);
        }

        public void WriteVerboseTrace(TraceEventId eventId, string msg)
        {
            WriteTrace(TraceEventType.Verbose, eventId, msg);
        }

        public void WriteVerboseTrace(TraceEventId eventId, string msg, params object[] args)
        {
            WriteTrace(TraceEventType.Verbose, eventId, msg, args);
        }

        public void WriteInformationTrace(TraceEventId eventId, string msg)
        {
            WriteTrace(TraceEventType.Information, eventId, msg);
        }

        public void WriteInformationTrace(TraceEventId eventId, string msg, params object[] args)
        {
            WriteTrace(TraceEventType.Information, eventId, msg, args);
        }

        public void WriteErrorTrace(TraceEventId eventId, string msg)
        {
            WriteTrace(TraceEventType.Error, eventId, msg);
        }

        public void WriteErrorTrace(TraceEventId eventId, string msg, params object[] args)
        {
            WriteTrace(TraceEventType.Error, eventId, msg, args);
        }

        public void WriteErrorTrace(TraceEventId eventId, Exception exception)
        {
            WriteTrace(TraceEventType.Error, eventId, exception.ToString());
        }

        #endregion

        #region Private methods

        private void InitializeDefaultSource()
        {
            source = new TraceSource(RootSourceName);
            source.Switch = new SourceSwitch(RootSourceName);
            source.Switch.Level = SourceLevelFromString(ConfigReader<string>.GetSetting(RootSourceName));
        }

        private SourceLevels SourceLevelFromString(string str)
        {
            SourceLevels lvl;
            try
            {
                lvl = (SourceLevels)Enum.Parse(typeof(SourceLevels), str, true);
            }
            catch (ArgumentException)
            {
                // Invalid value - just default to off.
                lvl = SourceLevels.Off;
            }
            return lvl;
        }

        private StringBuilder GetExceptionMessageWithStackTrace(Exception exception)
        {
            StringBuilder errorMessage = new StringBuilder();
            
            if (exception == null)
            {
                return  errorMessage.AppendLine("exception is null");
            }
           
            errorMessage.AppendLine("Exception log starts.......");
            WebException webException = exception as WebException;
            
            if (webException != null && webException.Status == WebExceptionStatus.ProtocolError)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                errorMessage.AppendLine(string.Format("{0}:{1}", "StatusCode", response.StatusCode));
                errorMessage.AppendLine(string.Format("{0}:{1}", "StatusDescription", response.StatusDescription));
            }
            else
            {
                errorMessage.AppendLine(string.Format("{0}:{1}", "Type", exception.GetType().FullName));
                errorMessage.AppendLine(string.Format("{0}:{1}", "Message",exception.Message));
                errorMessage.AppendLine(string.Format("{0}:{1}", "Stack", exception.StackTrace));
                errorMessage.AppendLine(string.Format("{0}:{1}", "Source", exception.Source));
            }
            
            return errorMessage;
          }

        #endregion
    }
}
