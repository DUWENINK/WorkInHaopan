using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DUWENINKHopenTools.Entity;

namespace DUWENINKHopenTools.Attribute
{
    public class MapLisAttribute: System.Attribute
    {
        private List<Map> _mapList;

        public MapLisAttribute(List<Map> mapList)
        {
            _mapList = mapList;
        }
        public bool IsValid()
        {

        }
    }
}
