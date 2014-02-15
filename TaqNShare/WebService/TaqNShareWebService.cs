using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.WebService
{
    class TaqNShareWebService
    {
        public ServiceTaqnshareClient WebService { get; set; }

        public TaqNShareWebService()
        {
            WebService = new ServiceTaqnshareClient();
        }
       
    }
}
