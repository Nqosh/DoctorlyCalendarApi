using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Abstractions
{
    public interface IUnitOfWork 
    { 
        Task SaveChangesAsync(CancellationToken ct); 
    }
}
