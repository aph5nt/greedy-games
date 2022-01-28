using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Contracts
{
    public class Response<T>
    {
        public Response(T data)
        {
            Data = data;
            Errors = new List<string>();
        }

        public Response(List<string > errors)
        {
            Errors = errors;
        }
        
        public T Data { get; }
        public List<string> Errors { get; }
        public bool IsValid => !Errors.Any();
    }

    public class Response
    {
        public Response()
        {
            Errors = new string[] { };
        }

        public Response(params string[] errors)
        {
            Errors = errors;
        }
        public Object Data { get; set; }
        public string[] Errors { get; set; }
        public bool IsValid  => !Errors.Any();
    }
}