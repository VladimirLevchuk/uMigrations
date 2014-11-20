using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using log4net;
using log4net.Core;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using umbraco.webservices;
using uMigrations.Metadata;

namespace uMigrations
{
    public class MigrationsApi : IMigrationsApi
    {
        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected IMigrationTransactionProvider MigrationTransactionProvider { get; private set; }
        protected ILog Log { get; private set; }

        public MigrationsApi(IContentMigrationService contentMigrationService, IMigrationTransactionProvider migrationTransactionProvider, ILog log)
        {
            ContentMigrationService = contentMigrationService;
            MigrationTransactionProvider = migrationTransactionProvider;
            Log = log;
        }

        //protected virtual void ExecuteAction(IMigrationAction action)
        //{
        //    try
        //    {
        //        Log.InfoFormat("Starting action execution. Action: {0}", action.ToString());
        //        action.Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.ErrorFormat("Action execution failed: '{0}'. Error Details: {1}", action, ex);
        //        throw;
        //    }
        //}

        protected virtual void ValidateTransactionPresent()
        {
            if (!MigrationTransactionProvider.IsTransactionStarted())
            {
                throw new InvalidOperationException("Migration action must be run whithin transaction scope. ");
            }            
        }

        public virtual void MoveProperty(string sourceTypeAlias, string destinationTypeAlias, 
            string propertyAlias, string tabName = null)
        {
            var contentToUpdate = ContentMigrationService.GetContentOfType(sourceTypeAlias).ToList();

            // check content supports both source and destination type
            var migrationProblems = new List<Exception>();

            //// todo: use parralel execution here

            foreach (var content in contentToUpdate)
            {
                if (!ContentMigrationService.IsContentOfType(content, destinationTypeAlias))
                {
                    string message = string.Format("Content item #{0} of type '{1}' is not of type '{2}'", content.Id, 
                        content.ContentType.Alias, destinationTypeAlias);
                    migrationProblems.Add(new InvalidOperationException(message));
                }
            }

            if (migrationProblems.Any())
            {
                var message = string.Format("Migration error: Unable to move property '{0}' from type '{1}' to type '{2}'",
                    propertyAlias, sourceTypeAlias, destinationTypeAlias);
                throw new AggregateException(message, migrationProblems);
            }

            // ok, we're good to go

            // validate if there is an active transaction
            ValidateTransactionPresent();

            var sourceContentType = ContentMigrationService.GetContentType(sourceTypeAlias);
            var destinationContentType = ContentMigrationService.GetContentType(destinationTypeAlias);

            var oldPropertyType = sourceContentType.PropertyTypes.FirstOrDefault(x => x.Alias == propertyAlias);

            if (oldPropertyType == null)
            {
                var message = string.Format("Property '{0}' is not found in type '{1}' ",
                    propertyAlias, sourceTypeAlias);
                throw new InvalidOperationException(message);
            }

            var oldPropertyAlias = RenamePropertyForDeletion(oldPropertyType, propertyAlias);
            var newProperty = CopyPropertyType(propertyAlias, oldPropertyType);
            //var mandatory = newProperty.Mandatory; // do we need to handle mandatory fields separately?
            //newProperty.Mandatory = false;
            
            if (tabName != null)
            {
                // todo ? do we need to create a new property group ?
                destinationContentType.AddPropertyType(newProperty, tabName);
            }
            else
            {
                destinationContentType.AddPropertyType(newProperty);
            }

            ContentMigrationService.UpdateContentTypes(sourceContentType, destinationContentType);

            //// todo: use parralel execution here
            foreach (var content in contentToUpdate)
            {
                var value = content.GetValue(oldPropertyAlias);
                content.SetValue(propertyAlias, value);

                ContentMigrationService.UpdateContent(content);
            }

            if (sourceContentType.RemoveContentType(oldPropertyAlias))
            {
                ContentMigrationService.UpdateContentTypes(sourceContentType);
            }
        }

        private PropertyType CopyPropertyType(string propertyAlias, PropertyType propertyType)
        {
            var dtd = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId);
            var result = new PropertyType(dtd)
            {
                Alias = propertyAlias,
                Description = propertyType.Description,
                Mandatory = propertyType.Mandatory,
                SortOrder = propertyType.SortOrder,
                ValidationRegExp = propertyType.ValidationRegExp,
                Name = propertyType.Name
            };

            return result;
        }

        private string RenamePropertyForDeletion(PropertyType propertyType, string propertyAlias)
        {
            if (propertyType == null) throw new ArgumentNullException("propertyType");
            propertyType.Alias = propertyAlias + "_" + MigrationRuntimeId;
            return propertyType.Alias;
        }

        private readonly Lazy<string> _migrationRuntimeId = new Lazy<string>(() => DateTime.Now.ToString("s").Replace("-", "_").Replace(":", "_"));

        public virtual string MigrationRuntimeId
        {
            get { return _migrationRuntimeId.Value; }
        }
        
    }

    public interface IMigrationAction
    {
        string ToString();
        void Run();
    }
}