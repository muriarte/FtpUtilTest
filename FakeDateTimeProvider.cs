using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegraSyncTest
{
    class FakeDateTimeProvider:FtpUtil.IDateTimeProvider
    {
        DateTime _date;

        private FakeDateTimeProvider() { }

        public FakeDateTimeProvider(DateTime date) {
            _date = date;
        }
        
        DateTime FtpUtil.IDateTimeProvider.GetCurrentDateTime() {
            return _date;
        }
    }
}
