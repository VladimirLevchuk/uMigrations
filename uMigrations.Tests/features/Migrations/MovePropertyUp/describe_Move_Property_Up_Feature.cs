using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using NSpec.Domain;
using Umbraco.Core;
using uMigrations.Tests.features.Migrations;
using uMigrations.Tests.Models.Db;
using Vega.USiteBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder.Contracts;
using Vega.USiteBuilder.Repositories;

namespace uMigrations.Tests.features.MovePropertyUp
{
    // ReSharper disable ConvertToLambdaExpression
    // ReSharper disable ConvertClosureToMethodGroup

    [Tag("acceptance")]
    class describe_Move_Property_Up_Feature : migrations_feature
    {
        private DbTableRepository _repo;

        void before_all()
        {
            InitializeDb();
        }

        void after_all()
        {
            FinalizeDb();
        }

        public class TestMigration1 : MigrationStepBase
        {
            protected override void Apply(FluentMigrationStepDefinition migration)
            {
                migration.MoveProperty("level2Prop2").ToBaseType("Level1").FromTypes("Level2");
            }
        }

        void describe_Move_Property_Up_with_no_content()
        {
            beforeAll = () =>
            {
                CreateDbSchema();
                AddDocumentTypes(typeof(Models.Level1), typeof(Models.Level2));
            };

            afterAll = () =>
            {
                DeleteDbSchema();
            };

            context["when simple migration applied"] = () =>
            {
                before = () =>
                {
                    SetupMigrationContext("test");
                };

                act = () =>
                {
                    RunMigration("SimpleMigration", migration =>
                    {
                        migration.MoveProperty("level2Prop2").ToBaseType("Level1").FromTypes("Level2");
                    });
                };

                it["property exists in the destination"] = () =>
                {
                    MigrationContext
                        .ContentMigrationService
                        .GetContentType("Level1")
                        .PropertyTypes
                        .FirstOrDefault(x => x.Alias == "level2Prop2")
                        .Should().NotBeNull();
                };

                it["and removed from the source"] = () =>
                {
                    MigrationContext
                        .ContentMigrationService
                        .GetContentType("Level2")
                        .PropertyTypes
                        .FirstOrDefault(x => x.Alias == "level2Prop2")
                        .Should().BeNull();
                };
            };
        }

        void describe_Move_Property_Up_with_content()
        {
            int item1 = 0;
            int item2 = 0;

            beforeAll = () =>
            {
                CreateDbSchema();
                AddDocumentTypes(typeof(Models.Level1), typeof(Models.Level2));

                var repo = DocumentRepository.Current;

                var level1Content = new Models.Level1
                {
                    Name = "level1 content",
                    Level1Prop1 = "one",
                    Level1Prop2 = "two",
                };

                repo.Save(level1Content);

                item1 = level1Content.Id;

                var level2Content = new Models.Level2
                {
                    Name = "level2 content",
                    Level1Prop1 = "three",
                    Level1Prop2 = "four",
                    Level2Prop1 = "five",
                    Level2Prop2 = "six",
                    ParentId = level1Content.Id
                };

                item2 = level2Content.Id;
                repo.Save(level2Content);
            };

            afterAll = () =>
            {
                DeleteDbSchema();
            };

            context["when simple migration applied"] = () =>
            {
                before = () =>
                {
                    SetupMigrationContext("test");
                };

                act = () =>
                {
                    RunMigration("SimpleMigration", migration =>
                    {
                        migration.MoveProperty("level2Prop2").ToBaseType("Level1").FromTypes("Level2");
                    });
                };

                it["property exists in the destination"] = () =>
                {
                    MigrationContext
                        .ContentTypeService
                        .GetContentType("Level1")
                        .PropertyTypes
                        .FirstOrDefault(x => x.Alias == "level2Prop2")
                        .Should().NotBeNull();

                };

                it["and removed from the source"] = () =>
                {
                    MigrationContext
                        .ContentTypeService
                        .GetContentType("Level2")
                        .PropertyTypes
                        .FirstOrDefault(x => x.Alias == "level2Prop2")
                        .Should().BeNull();
                };

                it["and existing data still present"] = () =>
                {
                    var repo = DocumentRepository.Current;
                    var level2Content = repo.GetById<Models.MigratedLevel2>(item2);

                    level2Content.Level2Prop2.Should().Be("");
                };

                it["and new field is empty"] = () =>
                {
                    var repo = DocumentRepository.Current;
                    var level1Content = repo.GetById<Models.MigratedLevel1>(item1);
                    level1Content.Level2Prop2.Should().BeNullOrEmpty();
                };
            };           
        }
    }

    // ReSharper restore ConvertToLambdaExpression
    // ReSharper restore ConvertClosureToMethodGroup
}
