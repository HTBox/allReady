﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestError
    {
        public string ProviderId { get; set; }
        public string Reason { get; set; }
    }
}
