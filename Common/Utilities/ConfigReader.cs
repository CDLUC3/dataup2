﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    /// <summary>
    /// Helper class for reading a value from Application Configuration file.
    /// </summary>
    /// <typeparam name="T">
    /// Type parameter.
    /// </typeparam>
    public static class ConfigReader<T>
    {
        /// <summary>
        ///  Helper method to read a value from config file. Returns user specified default value
        ///  in case any failure reading the config value.
        /// </summary>
        /// <param name="configKey">Configuration key name.</param>
        /// <param name="defaultValue">Default value to bet set in case of failure.</param>
        /// <returns>
        /// A valid value from configuration file.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Avoids a new on every call"), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception.")]
        public static T GetConfigSetting(string configKey, T defaultValue)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[configKey];
                if (!string.IsNullOrEmpty(value))
                {
                    return (T)Convert.ChangeType(value, defaultValue.GetType(), CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                // -
                // Swallow the exception to return a default value
                // -
            }

            return defaultValue;
        }

        /// <summary>
        ///  Helper method to read a value from config file (application config or cloud config file). 
        /// </summary>
        /// <param name="settingKey">Configuration key name.</param>
        /// <returns>
        /// A valid value from configuration file (application config or cloud config file).
        /// </returns>
        //// [WindowsAzureHostingPermission(SecurityAction.LinkDemand, Unrestricted = true)] 
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Avoids a new on every call"), SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "CA2122 conflicts with CA2135.")]
        public static T GetSetting(string settingKey)
        {
            return GetSetting(settingKey, default(T));
        }

        /// <summary>
        ///  Helper method to read a value from config file (application config or cloud config file). 
        /// </summary>
        /// <param name="settingKey">Configuration key name.</param>
        /// <param name="defaultValue">Default value to bet set in case of failure.</param>
        /// <returns>
        /// A valid value from configuration file (application config or cloud config file).
        /// </returns>
        //// [WindowsAzureHostingPermission(SecurityAction.LinkDemand, Unrestricted = true)] 
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Avoids a new on every call"), SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "CA2122 conflicts with CA2135.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception.")]
        public static T GetSetting(string settingKey, T defaultValue)
        {
            string settingValue = string.Empty;
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    settingValue = RoleEnvironment.GetConfigurationSettingValue(settingKey);
                }
                else
                {
                    settingValue = ConfigurationManager.AppSettings[settingKey];
                }
            }
            catch
            {
                // -
                // Swallow the exception to return a default value
                // -
                settingValue = ConfigurationManager.AppSettings[settingKey];
            }

            if (!string.IsNullOrEmpty(settingValue))
            {
                return (T)Convert.ChangeType(settingValue, typeof(T), CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }
        
        /// <summary>
        /// Helper method to return the RepositoryConfig value.
        /// </summary>
        /// <param name="baseRepositoryName">Base Repository Name.</param>
        /// <param name="key">Key Name.</param>
        /// <returns>Config Value.</returns>
        public static T GetRepositoryConfigValues(string baseRepositoryName,string key)
        {
            string settingValue = string.Empty;
            NameValueCollection configValues = new NameValueCollection() ;
            configValues = (NameValueCollection)ConfigurationManager.GetSection(baseRepositoryName);
            settingValue = configValues[key];
            return (T)Convert.ChangeType(settingValue, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
