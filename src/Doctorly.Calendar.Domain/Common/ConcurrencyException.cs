using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Domain.Common
{
    public class ConcurrencyException(string message) : Exception(message);
}
