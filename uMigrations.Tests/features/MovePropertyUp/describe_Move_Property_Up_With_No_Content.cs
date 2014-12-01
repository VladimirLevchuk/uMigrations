using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using NSpec.Domain;
using uMigrations.Tests.Models.Db;
using Vega.USiteBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder.Contracts;

namespace uMigrations.Tests.features.MovePropertyUp
{
    public abstract class migrations_feature : feature
    {
        protected MigrationContext MigrationContext
        {
            get { return MigrationContext.Current; }
            set { MigrationContext.Current = value; }
        }
    }

    [Tag("acceptance")]
    class describe_Move_Property_Up_Feature : migrations_feature
    {
        private DbTableRepository _repo;

        void before_all()
        {
            _repo = new DbTableRepository(Db);
            _repo.CreateSchema();
        }

        void after_all()
        {
            _repo.DeleteAll();
            _repo = null;
        }

        public class TestMigration1 : MigrationStepBase
        {
            protected override void Apply(FluentMigrationStepDefinition migration)
            {
                migration.MoveProperty("level2Prop2").ToBaseType("Level1").FromTypes("Level2");
            }
        }

        void describe_Move_Property_Up()
        {
            context["With no content"] = () =>
            {
                beforeAll = () => new TestDocumentTypeManager().SynchronizeDocumentTypes(new[] {typeof (Models.Level1), typeof(Models.Level2)});

                context["when simple migration applied"] = () =>
                {
                    before = () =>
                    {
                        MigrationContext = MigrationTestUtil.CreateMigrationContext(new MigrationsSettings
                        {
                              SystemUserId = 0,
                              EmulateMigrations = false,
                              MigrationRuntimeId = "test",
                              SkipMigrations = false
                        });

                        MigrationContext.Runner.Apply(new CustomMigration(new TestMigration1()));
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
            };
        }
    }
}
