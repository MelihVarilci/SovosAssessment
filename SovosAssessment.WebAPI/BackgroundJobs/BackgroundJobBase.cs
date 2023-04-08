using Microsoft.Extensions.Logging.Abstractions;
using SovosAssessment.Infrastructure.Persistence.Repositories;

namespace SovosAssessment.WebAPI.BackgroundJobs
{
    public abstract class BackgroundJobBase<TArgs> : IBackgroundJobBase<TArgs>
    {
        /// <summary>
        /// Reference to <see cref="IUnitOfWork"/>.
        /// </summary>
        public IUnitOfWork UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new Exception("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }

        private IUnitOfWork _unitOfWorkManager;

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BackgroundJobBase()
        {
            Logger = NullLogger.Instance;
        }
    }
}
