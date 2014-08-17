using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balancer.Common.Utils
{
    interface IClonable<out T>
    {
        T Clone();
    }
}
