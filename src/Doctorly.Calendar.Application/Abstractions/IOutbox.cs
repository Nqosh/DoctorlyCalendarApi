using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Abstractions
{
    public interface IOutbox
    {
        void Add(string type, object payload);
    }
}
