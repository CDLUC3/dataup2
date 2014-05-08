// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
      
namespace Microsoft.Research.DataOnboarding.TestUtilities
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// This class contains helper methods for running tests
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Helper
    {
        /// <summary>
        /// This method creates a new SQL CE database from Entity framework context
        /// </summary>
        /// <param name="context">Entity framework context</param>
        /// <param name="databaseFilePath">File path</param>
        /// <returns>Sql Ce database file path</returns>
        /// <remarks>
        /// In future look at refactoring this method to take an additional 'overwrite' flag, and create new database optional 
        /// </remarks>
        public static void CreateSqlCeDataBaseFromEntityFrameworkDbContext(DbContext context, string databaseFilePath)
        {
            // If the database file already exists then delete it.
            if (File.Exists(databaseFilePath))
            {
                File.Delete(databaseFilePath);
            }

            // It is required to set the 'DefaultConnectionFactory' static property to SqlCeConnectionFactory instance
            // to generate database. Refer online documentation.
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");

            context.Database.Create();
        }
    }
}
