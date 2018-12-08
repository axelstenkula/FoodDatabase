using System.Collections.Generic;
using System.Net;
using FoodDatabase;
using Newtonsoft.Json;

namespace Api
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiError : Error
    {
        // Http status code
        [JsonProperty("status")]
        public HttpStatusCode HttpStatusCode { get; private set; }

        // More specific internal error code to aid in debugging.
        [JsonProperty("code")]
        public string ErrorCode
        {
            get
            {
                return this.Code.ToString();//ToFriendlyString();
            }
        }

        // Error message that can be shown to a user in a GUI.
        [JsonProperty("userMessage")]
        public string UserMessage { get; private set; }

        // Extra message for developers to help debug the issue.
        [JsonProperty("developerMessage")]
        public string DeveloperMessage { get; private set; }

        [JsonProperty("moreInfo")]
        public string AdditionalInfo
        {
            get
            {
                return "";
            }
        }

        [JsonProperty("causedBy", NullValueHandling = NullValueHandling.Ignore)]
        public new List<IError> CausedBy
        {
            get { return base._causedBy; }
            // set { base._causedBy = value; }
        }

        public List<IError> Errors { get; set; }

        public ApiError(string message) : base(message)
        {
            this.HttpStatusCode = HttpStatusCode.OK;
            this.UserMessage = "";
            this.DeveloperMessage = "";
            this.Errors = new List<IError>();
        }

        public ApiError(ErrorCode code, string message) : base(code, message)
        {
            this.HttpStatusCode = HttpStatusCode.OK;
            this.UserMessage = "";
            this.DeveloperMessage = "";
            this.Errors = new List<IError>();
        }


        public static ApiError NewApiError(ErrorCode code)
        {
            ApiError error = new ApiError(code, "");

            switch (code)
            {
                // case Error:
                //     error.HttpStatusCode = HttpStatusCode.InternalServerError;
                //     error.UserMessage = "Internal Server Error.";
                //     error.DeveloperMessage = "Internal Server Error.";
                //     break;
                default:
                    error.HttpStatusCode = HttpStatusCode.InternalServerError;
                    error.UserMessage = "Internal Server Error.";
                    error.DeveloperMessage = "Internal Server Error.";
                    break;
            }

            return error;
        }

        public static ApiError NewFromIError(IError error)
        {
            var apiError = NewApiError(error.Code);

            switch (error.Code)
            {
                default:
                    break;
            }
            foreach (var errorCause in error.CausedBy)
            {
                apiError._causedBy.Add(errorCause);
            }

            return apiError;
        }

        // public static ApiError NewFromIdentityError(IdentityError error)
        // {
        //     var apiError = NewApiError(error.Code);
        //     apiError.DeveloperMessage = error.Message;
        //     // FIXME Should we support causedBy in api response?
        //     return apiError;
        // }
    }
}
