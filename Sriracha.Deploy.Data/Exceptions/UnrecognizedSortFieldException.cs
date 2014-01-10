using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Exceptions
{
    public class UnrecognizedSortFieldException<T> : Exception
    {
        public UnrecognizedSortFieldException(ListOptions listOptions) : base(FormatMessage(listOptions))
        {

        }

        private static string FormatMessage(ListOptions listOptions)
        {
            if(listOptions == null)
            {
                return "No ListOptions provided to sort type " + typeof(T).FullName;
            }
            return string.Format("Unrecognized sort field {0} for type {1}", listOptions.SortField, typeof(T).FullName);
        }
    }
}
