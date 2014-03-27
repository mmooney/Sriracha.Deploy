using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MMDB.Azure.Management
{
    /// <summary>
    /// http://stackoverflow.com/a/1564727/203479
    /// </summary>
    [Obsolete("Move to MMDB.Shared")]
    internal sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
