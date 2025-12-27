using System;
using System.Collections.Generic;
using System.Text;

namespace FinCorralApi.Shared.Results
{
    public sealed record Error(string Code, string Message)
    {
        public static Error None => new(string.Empty, string.Empty);
    }
}
