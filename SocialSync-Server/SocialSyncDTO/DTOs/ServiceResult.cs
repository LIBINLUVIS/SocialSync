﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncDTO.DTOs
{
     public class ServiceResult<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

    }
}
