using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunctions
{
    public class ReconciliationContext
    {
        public DateTime StartTime { get; set; }
        public RetrySetting RetrySetting { get; set; }
    }
}
