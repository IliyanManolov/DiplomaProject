using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Lib.Observability.Models;

public record LogModel
{
    public string ts { get; set; }
    public string v { get; set; }
    public string msg { get; set; }
    public string ctx { get; set; }
    public string st { get; set; }
    public string err { get; set; }
    public string errType { get; set; }
    public string tr { get; set; }
    public IEnumerable<ExModel> innerEx { get; set; }
    public IEnumerable<PropModel> prop { get; set; }
    public IEnumerable<PropModel> propLong { get; set; }
}
