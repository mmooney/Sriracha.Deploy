﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public interface ISqlConnectionInfo
    {
        PetaPoco.Database GetDB();
    }
}