namespace SetElementCustomProperty_1
{
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Skyline.DataMiner.Automation;

    internal static class IDmsElementExtensions
    {
        public static string GetPropertyValue(this IDmsElement element, string propertyName)
        {
            return element.Properties[propertyName]?.Value ?? throw new InvalidOperationException($"Unable to find property {propertyName} on element {element.Id}");
        }

        public static void SetPropertyValue(this IDmsElement element, string propertyName, string propertyValue)
        {
            var property = element.Properties[propertyName] as IWritableProperty ?? throw new InvalidOperationException($"Unable to find property {propertyName} on element {element.Id}");

            property.Value = propertyValue;

            element.Update();
        }

        public static void SetParameter<T>(this IDmsElement element, int parameterId, T valueToSet)
        {
            element.GetStandaloneParameter<T>(parameterId).SetValue(valueToSet);
        }

        public static T GetParameter<T>(this IDmsElement element, int parameterId)
        {
            return element.GetStandaloneParameter<T>(parameterId).GetValue();
        }

        public static T GetParameterByPrimaryKey<T>(this IDmsElement element, int tablePid, int columnPid, string primaryKey)
        {
            var table = element.GetTable(tablePid);
            var column = table.GetColumn<T>(columnPid);
            var value = column.GetValue(primaryKey, KeyType.PrimaryKey);

            return value;
        }

        public static void SetParameterByPrimaryKey<T>(this IDmsElement element, int tablePid, int columnPid, string primaryKey, T valueToSet)
        {
            var table = element.GetTable(tablePid);
            var column = table.GetColumn<T>(columnPid);
            column.SetValue(primaryKey, KeyType.PrimaryKey, valueToSet);
        }

        public static IDmsElement GetElement(string elementName)
        {
            return Skyline.DataMiner.Automation.Engine.SLNetRaw.GetDms().GetElement(elementName);
        }
    }
}
