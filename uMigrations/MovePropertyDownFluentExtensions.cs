using System.Collections.Generic;

namespace uMigrations
{
    public static class MovePropertyDownFluentExtensions
    {
        public class MovePropertyToChildTypesClause
        {
            public FluentExtensions.MovePropertyClause MovePropertyClause { get; set; }
            public List<string> DestinationContentTypes { get; set; }
            public object DefaultValue { get; set; }
            public bool IsDefaultSet { get; set; }
            public bool IsMandatory { get; set; }
            public string TabName { get; set; }
            public INoValueAnalyzer NoValueAnalyzer { get; set; }

            public MovePropertyToChildTypesClause(FluentExtensions.MovePropertyClause movePropertyClause, List<string> destinationTypes)
            {
                MovePropertyClause = movePropertyClause;
                DestinationContentTypes = destinationTypes;
            }

            public MovePropertyToChildTypesClause Mandatory()
            {
                IsMandatory = true;
                return this;
            }

            public MovePropertyToChildTypesClause WithDefault(object value)
            {
                DefaultValue = value;
                IsDefaultSet = true;
                return this;
            }

            public MovePropertyToChildTypesClause WithDefault(object value, INoValueAnalyzer noValueAnalyzer)
            {
                DefaultValue = value;
                IsDefaultSet = true;
                NoValueAnalyzer = noValueAnalyzer;
                return this;
            }

            public MovePropertyToChildTypesClause ToTab(string tabName)
            {
                TabName = tabName;
                return this;
            }

            public void FromType(string sourceType)
            {
                var parameters = new MovePropertyDownParameters(sourceType,
                    DestinationContentTypes,
                    MovePropertyClause.PropertyAlias,
                    TabName,
                    IsMandatory,
                    DefaultValue);

                var action = new MovePropertyDown(parameters, MigrationContext.Current);
                MovePropertyClause.Migration.Actions.Add(action);
            }
        }

        public static MovePropertyToChildTypesClause ToDerivedTypes(this FluentExtensions.MovePropertyClause movePropertyClause,
            string destinationType, params string[] destinationTypes)
        {
            var types = new List<string>();
            types.Add(destinationType);
            types.AddRange(destinationTypes);
            
            return new MovePropertyToChildTypesClause(movePropertyClause, types);
        }
    }
}