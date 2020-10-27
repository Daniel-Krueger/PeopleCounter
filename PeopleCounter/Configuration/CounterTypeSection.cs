using System.Configuration;

namespace PeopleCounter
{
    public class CounterTypeSection : ConfigurationSection
    {
        [ConfigurationProperty("counterTypes")]
        public CounterTypeCollection CounterTypes
        {
            get { return ((CounterTypeCollection)(base["counterTypes"])); }
            set { base["counterTypes"] = value; }
        }
    }
}
