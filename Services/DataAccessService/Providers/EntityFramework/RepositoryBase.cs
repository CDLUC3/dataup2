// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) 2013 Microsoft Corporation
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Research.DataOnboarding.DataAccessService.Providers.EntityFramework
{
    using Microsoft.Research.DataOnboarding.DataAccessService;
    using Microsoft.Research.DataOnboarding.Utilities;

    /// <summary>
    /// This is the base class for all repositories that leverage
    /// entity framework
    /// </summary>
    public abstract class RepositoryBase
    {
        private IDataOnboardingContext context;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        protected RepositoryBase(IUnitOfWork onboardingContext)
        {
            Check.IsNotNull<IUnitOfWork>(onboardingContext, "onboardingContext");
            this.context = onboardingContext as IDataOnboardingContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity framework database context to derived types
        /// </summary>
        protected IDataOnboardingContext Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion
    }
}
