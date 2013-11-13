using dropkick.Configuration.Dsl;
using dropkick.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DropkicK.Extensions.Configuration.Dsl.Files
{
    public static class Extension
    {
        public static void OpenFolderShareWithAuthentication(this ProtoServer protoServer, string folderName, string userName, string password)
        {
            var task = new OpenFolderShareAuthenticationProtoTask(folderName, userName, password);
            protoServer.RegisterProtoTask(task);
        }
    }
}
