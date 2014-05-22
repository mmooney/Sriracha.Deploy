using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerRazorTemplateRepository : IRazorTemplateRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerRazorTemplateRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public RazorTemplate GetTemplate(string viewName, string defaultViewData)
        {
            if(string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException("viewName");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<RazorTemplate>("FROM RazorTemplate WHERE ViewName=@0", viewName);
                if(item == null)
                {
                    item = new RazorTemplate
                    {
                        ViewName = viewName,
                        ViewData = defaultViewData
                    };
                }
                return item;
            }
        }

        public RazorTemplate SaveTemplate(string viewName, string viewData)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException("viewName");
            }
            if(string.IsNullOrEmpty(viewData))
            {
                throw new ArgumentNullException("viewData");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<RazorTemplate>("FROM RazorTemplate WHERE ViewName=@0", viewName);
                if (item == null)
                {
                    item = new RazorTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        ViewName = viewName,
                        ViewData = viewData
                    };
                    item.SetCreatedFields(_userIdentity.UserName);
                    db.Insert("RazorTemplate", "ID", false, item);
                }
                else 
                {
                    item.ViewData = viewData;
                    item.SetUpdatedFields(_userIdentity.UserName);
                    db.Update("RazorTemplate", "ID", item, item.Id);
                }
                return item;
            }
        }

        public RazorTemplate DeleteTemplate(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException("viewName");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<RazorTemplate>("FROM RazorTemplate WHERE ViewName=@0", viewName);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(RazorTemplate), "ViewName", viewName);
                }
                db.Execute("DELETE FROM RazorTemplate WHERE ViewName=@0", viewName);
                return item;
            }
        }
    }
}
