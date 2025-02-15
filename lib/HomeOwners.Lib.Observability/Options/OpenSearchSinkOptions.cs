using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Lib.Observability.Options;

public class OpenSearchSinkOptions
{
    /// <summary>
    /// Time between batch emission
    /// </summary>
    public long BatchTime { get; set; } = 60;
    public int BatchSize { get; set; } = 50;

    public string Index { get; set; }
    public string ConnectionUrl { get; set; }
    public string Password { get; set; }

    public string User { get; set; }
}

