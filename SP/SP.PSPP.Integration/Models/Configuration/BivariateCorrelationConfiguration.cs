﻿using System.Collections.Generic;

namespace SP.PSPP.Integration.Models.Configuration
{
    public class BivariateCorrelationConfiguration : IConfiguration
    {
        public IEnumerable<string> Variables { get; set; }

        public IEnumerable<string> Headers
        {
            get
            {
                return Variables;
            }
        }
    }
}