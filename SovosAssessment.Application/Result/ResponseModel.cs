using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SovosAssessment.Application.Result
{
    /// <summary>
    /// Class ResponseModel.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class ResponseModel<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModel{T}"/> class.
        /// </summary>
        /// <param name="data">The response data.</param>
        public ResponseModel(T data)
        {
            this.Result = data;
            this.Success = true;
            this.Error = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModel{T}"/> class.
        /// </summary>
        public ResponseModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModel{T}"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        public ResponseModel(string errorCode, string errorMessage, int statusCode)
        {
            this.Result = default(T);
            this.Success = false;
            this.Error = new ErrorModel
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        /// <value><c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public ErrorModel Error { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>The HTTP status code.</value>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the response data.
        /// </summary>
        /// <value>The response data.</value>
        public T Result { get; set; }
    }

    /// <summary>
    /// Class ErrorModel.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }
    }
}
