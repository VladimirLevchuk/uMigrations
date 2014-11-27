using System;
using System.Collections.Generic;

namespace uMigrations
{
    public interface INoValueAnalyzer
    {
        bool NoValue(object value);
    }

    public class DefaultNoValueAnalyzer : INoValueAnalyzer
    {
        public bool NoValue(object value)
        {
            return value == null;
        }
    }

    public class NoValueWhenNullOrEmpty : INoValueAnalyzer
    {
        public bool NoValue(object value)
        {
            var stringValue = (string) value;

            return string.IsNullOrEmpty(stringValue);
        }
    }

    public static class MovePropertyUpFluentExtensions
    {
        public class MovePropertyToBaseTypeClause
        {
            public FluentExtensions.MovePropertyClause MovePropertyClause { get; set; }
            public string DestinationContentType { get; set; }
            public object DefaultValue { get; set; }
            public bool IsDefaultSet { get; set; }
            public bool IsMandatory { get; set; }
            public string TabName { get; set; }
            public INoValueAnalyzer NoValueAnalyzer { get; set; }

            public MovePropertyToBaseTypeClause(FluentExtensions.MovePropertyClause movePropertyClause, 
                string destinationContentType)
            {
                MovePropertyClause = movePropertyClause;
                DestinationContentType = destinationContentType;
            }

            public MovePropertyToBaseTypeClause Mandatory()
            {
                IsMandatory = true;
                return this;
            }

            public MovePropertyToBaseTypeClause WithDefault(object value)
            {
                DefaultValue = value;
                IsDefaultSet = true;
                return this;
            }

            public MovePropertyToBaseTypeClause WithDefault(object value, INoValueAnalyzer noValueAnalyzer)
            {
                DefaultValue = value;
                IsDefaultSet = true;
                NoValueAnalyzer = noValueAnalyzer;
                return this;
            }

            public MovePropertyToBaseTypeClause ToTab(string tabName)
            {
                TabName = tabName;
                return this;
            }

            public void FromTypes(string sourceType, params string[] otherSourceTypes)
            {
                var sourceTypes = new List<string>();
                sourceTypes.Add(sourceType);
                sourceTypes.AddRange(otherSourceTypes);

                var parameters = new MovePropertyUpParameters(DestinationContentType, 
                    sourceTypes, 
                    MovePropertyClause.PropertyAlias,
                    TabName,
                    IsMandatory,
                    DefaultValue);

                var action = new MovePropertyUp(parameters, MigrationContext.Current);
                MovePropertyClause.Migration.Actions.Add(action);
            }
        }

        public static MovePropertyToBaseTypeClause ToBaseType(this FluentExtensions.MovePropertyClause movePropertyClause, 
            string destinationContentType)
        {
            return new MovePropertyToBaseTypeClause(movePropertyClause, destinationContentType);
        }
    }
}