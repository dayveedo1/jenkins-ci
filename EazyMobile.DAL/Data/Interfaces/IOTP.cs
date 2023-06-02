using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.Interfaces
{
    public interface IOTP
    {
        string GetCurrentOTP();
        string GetNextOTP();
    }
}
