using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client.QuerySequence
{
    public class QuerySequenceBase
    {
        protected readonly object GetNextQueryLockObject = new object();
    }
}
