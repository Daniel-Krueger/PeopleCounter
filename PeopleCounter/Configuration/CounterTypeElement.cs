using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PeopleCounter
{
    public class CounterTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("iconText", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string IconText
        {
            get { return (string)base["iconText"]; }
            set { base["iconText"] = value; }
        }
        [ConfigurationProperty("csvIdentifier", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string CsvIdentifier
        {
            get { return (string)base["csvIdentifier"]; }
            set { base["csvIdentifier"] = value; }
        }

        [ConfigurationProperty("backGroundColor", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string BackGroundColor
        {
            get { return (string)base["backGroundColor"]; }
            set { base["backGroundColor"] = value; }
        }

        [ConfigurationProperty("foreGroundColor", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string ForeGroundColor
        {
            get { return (string)base["foreGroundColor"]; }
            set { base["foreGroundColor"] = value; }
        }
    }
}
