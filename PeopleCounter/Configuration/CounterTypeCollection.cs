using System;
using System.Configuration;

namespace PeopleCounter
{
    [ConfigurationCollection(typeof(CounterTypeElement))]
    public class CounterTypeCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "counterType";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }


        public override bool IsReadOnly()
        {
            return false;
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new CounterTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CounterTypeElement)(element)).CsvIdentifier;
        }

        public CounterTypeElement this[int idx]
        {
            get
            {
                return (CounterTypeElement)BaseGet(idx);
            }
        }
    }
}
