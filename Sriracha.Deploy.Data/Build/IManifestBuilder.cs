using Sriracha.Deploy.Data.Dto.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Build
{
    public interface IManifestBuilder
    {
        FileManifest BuildFileManifest(byte[] fileData);
    }
}
