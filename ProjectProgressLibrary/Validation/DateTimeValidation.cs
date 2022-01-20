using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Validation
{
    public class DateTimeValidation
    {

        internal static DateTime GetDefaultDate()
        {
            string defaultDate = "13/05/2020 00:00:00 AM";

            DateTime outputDate = CreateDateFromString(defaultDate);

            return outputDate;
        }
    }
}
