using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Application.ValidationErrors.Base;

public class BaseValidationError : Exception
{
    public string Name = "ValidationError";

    public BaseValidationError(string message, string name) : base(message)
    {
        Name = name;
    }
}