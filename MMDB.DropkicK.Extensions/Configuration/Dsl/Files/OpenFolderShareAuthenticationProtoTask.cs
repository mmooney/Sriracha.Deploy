using dropkick;
using dropkick.FileSystem;
using dropkick.Tasks;
using MMDB.DropkicK.Extensions.Tasks.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.DropkicK.Extensions.Configuration.Dsl.Files
{
    public class OpenFolderShareAuthenticationProtoTask : BaseProtoTask
    {
        private readonly string _folderName;
        private readonly string _userName;
        private readonly string _password;

        public OpenFolderShareAuthenticationProtoTask(string folderName, string userName, string password)
        {
            _folderName = ReplaceTokens(folderName);
            _userName = userName;
            _password = password;
        }

        public override void RegisterRealTasks(dropkick.DeploymentModel.PhysicalServer server)
        {
            string to = server.MapPath(_folderName);

            var task = new OpenFolderShareAuthenticationTask(to, _userName, _password);
            server.AddTask(task);
        }
    }
}
