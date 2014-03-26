using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
    /// <summary>
    /// http://stackoverflow.com/a/1564727/203479
    /// </summary>
    [Obsolete("Move to MMDB.Shared")]
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
