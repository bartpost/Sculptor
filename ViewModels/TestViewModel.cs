using Sculptor.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace Sculptor.ViewModels
{
    class TestViewModel
    {
        public TestViewModel()
        {

        }

        Telerik.Windows.Data.ObservableItemCollection<ObjectModel> testItems = new Telerik.Windows.Data.ObservableItemCollection<ObjectModel>();
        Telerik.Windows.Data.ObservableItemCollection<ObjectModel> TestItems
        {
            get { return testItems; }
            set { testItems = value; }
        }
    }
}
