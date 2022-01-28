using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Model;

namespace Web.Models.NavigationViewModels
{
    public class NavModel
    {
        public List<string> Networks()
        {
            return Enum.GetNames(typeof(Network)).ToList();
        }
    }
}
