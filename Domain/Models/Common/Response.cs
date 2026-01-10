using Domain.Constants.AppEnum;

namespace Domain.Models.Common
{
	public class Response<T>
	{
        public ResponseResult Result { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public Response(ResponseResult result)
        {
            this.Result = result;
        }

        public Response(ResponseResult result, string? message, T? data, IDictionary<string, string[]>? errors)
        {
            this.Result = result;
            this.Message = message;
            this.Data = data;
            this.Errors = errors;
        }

        public Response(T data)
        {
            Result = ResponseResult.SUCCESS;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
    }
}

