using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    class ConnectionGenerator : Connection
    {
        public ConnectionGenerator(string connectionType) : base(connectionType)
        {
        }
    }
}
