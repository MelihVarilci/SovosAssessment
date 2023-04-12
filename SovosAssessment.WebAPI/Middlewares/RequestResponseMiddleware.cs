using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SovosAssessment.Application.Result;
using System.Net;
using System.Text;

namespace SovosAssessment.WebAPI.Middlewares
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            using (var responseBodyBufferStream = new MemoryStream())
            {
                //Store the original body stream in a local variable
                var responseBodyStream = context.Response.Body;

                try
                {
                    //Replace the context response with the newly created buffer as the original stream is not readable.
                    context.Response.Body = responseBodyBufferStream;

                    //Invoke the rest of the pipeline
                    await _next.Invoke(context); // Response bu satırda oluşuyor

                    //Reset the buffer so the content can be read
                    responseBodyBufferStream.Seek(0, SeekOrigin.Begin);

                    //Create a stream reader to be able to read the response
                    using (var bufferStreamReader = new StreamReader(responseBodyBufferStream))
                    {
                        var jsonSettings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
                            Formatting = Formatting.Indented,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };

                        //Read the body from the stream
                        var response = JsonConvert.DeserializeObject<object>(await bufferStreamReader.ReadToEndAsync(), jsonSettings);

                        //var response = new ResponseModel<object>()
                        //{
                        //    Success = true,
                        //    Error = null,
                        //    StatusCode = context.Response.StatusCode,
                        //    Result = JsonConvert.DeserializeObject<object>(await bufferStreamReader.ReadToEndAsync(), jsonSettings)
                        //};

                        //Reset the buffer
                        responseBodyBufferStream.Seek(0, SeekOrigin.Begin);

                        // Serialize the ResponseModel object into a JSON string
                        var responseJson = JsonConvert.SerializeObject(response, jsonSettings);

                        //// Convert the JSON string into a byte array
                        var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                        await responseBodyStream.WriteAsync(responseBytes);
                        await responseBodyStream.FlushAsync();

                        //Copy the content to the original stream and put it back
                        //await responseBodyBufferStream.CopyToAsync(responseBodyStream);
                    }
                }
                finally
                {
                    //Ensure original body stream is is written back to the response body even if an exception occurs in another middleware.
                    context.Response.Body = responseBodyStream;
                }
            }
        }
    }
}
