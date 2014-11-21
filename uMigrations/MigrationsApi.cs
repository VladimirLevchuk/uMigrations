using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper.Internal;
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

        protected virtual void ValidateContent()
        { }

        public virtual void MovePropertyUp(string sourceTypeAlias, string destinationTypeAlias, string propertyAlias, string tabName = null)
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

            sourceContentType.RemovePropertyType(oldPropertyAlias);
            ContentMigrationService.UpdateContentTypes(sourceContentType);
        }



        
    }

    public interface IMigrationAction
    {
        string ToString();
        void Validate();
        void Run();
    }

    public abstract class MovePropertyBase : IMigrationAction
    {
        public abstract void Validate();
        public abstract void Run();
    }

    public class MovePropertyDownParameters
    {
        public MovePropertyDownParameters(string sourceTypeAlias, IEnumerable<string> destinationTypes)
        {
            DestinationTypes = destinationTypes.ToList().AsReadOnly();
            SourceTypeAlias = sourceTypeAlias;
        }

        public string SourceTypeAlias { get; private set; }
        public IReadOnlyCollection<string> DestinationTypes { get; private set; }
    }

    public class MovePropertyUpParameters
    {
        public MovePropertyUpParameters(string destinationTypeAlias, IEnumerable<string> sourceTypes, string propertyAlias)
        {
            PropertyAlias = propertyAlias;
            SourceTypes = sourceTypes.ToList().AsReadOnly();
            DestinationTypeAlias = destinationTypeAlias;
        }

        public string DestinationTypeAlias { get; private set; }
        public IReadOnlyCollection<string> SourceTypes { get; private set; }
        public string PropertyAlias { get; private set; }
    }

    public interface IMigration
    {
        string MigrationRuntimeId { get; }
    }

    public abstract class MigrationBase : IMigration
    {
        private readonly Lazy<string> _migrationRuntimeId = new Lazy<string>(() => DateTime.Now.ToString("s").Replace("-", "_").Replace(":", "_"));

        public virtual string MigrationRuntimeId
        {
            get { return _migrationRuntimeId.Value; }
        }
    }

    public class MovePropertyUp : MovePropertyBase
    {
        public MovePropertyUp(MovePropertyUpParameters parameters, 
            IMigration migration,
            IContentMigrationService contentMigrationService, 
            IMigrationTransactionProvider migrationTransactionProvider, 
            ILog log)
        {
            Parameters = parameters;
            Migration = migration;
            Log = log;
            MigrationTransactionProvider = migrationTransactionProvider;
            ContentMigrationService = contentMigrationService;
            _contentToUpdate = new Lazy<List<IContent>>(GetContentToUpdate);
        }

        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected IMigrationTransactionProvider MigrationTransactionProvider { get; private set; }
        public MovePropertyUpParameters Parameters { get; private set; }
        protected IMigration Migration { get; private set; }
        protected ILog Log { get; private set; }

        private readonly Lazy<List<IContent>> _contentToUpdate;
        
        protected virtual List<IContent> GetContentToUpdate()
        {
            return ContentMigrationService.GetContentOfTypes(Parameters.SourceTypes).ToList();
        }

        protected virtual List<IContent> ContentToUpdate
        {
            get { return _contentToUpdate.Value; }
        }

        protected virtual void DoValidate(IEnumerable<string> sourceTypes, string destinationTypeAlias, string propertyAlias)
        {
            // check content supports both source and destination type
            var migrationProblems = new List<Exception>();

            var destinationContentType = ContentMigrationService.GetContentType(destinationTypeAlias);
            if (destinationContentType == null)
            {
                throw new Exception(string.Format("Destination Content type '{0}' not found. ", destinationTypeAlias));
            }

            // todo: measure performance and cache property types if needed
            foreach (var sourceType in sourceTypes)
            {
                var sourceContentType = ContentMigrationService.GetContentType(sourceType);
                if (sourceContentType == null)
                {
                    migrationProblems.Add(new Exception(string.Format("Content type '{0}' not found. ", sourceType)));
                    continue;
                }

                var property = ContentMigrationService.GetPropetyType(sourceContentType, propertyAlias);

                if (property == null)
                {
                    var message = string.Format("Property '{0}' is not found in type '{1}' ",
                        propertyAlias, sourceType);
                    migrationProblems.Add(new InvalidOperationException(message));
                }
            }

            foreach (var content in ContentToUpdate)
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
                var message = string.Format("Migration error: Unable to move property '{0}' to type '{1}'",
                    propertyAlias, destinationTypeAlias);
                throw new AggregateException(message, migrationProblems);
            }
        }

        public override void Validate()
        {
            DoValidate(Parameters.SourceTypes, Parameters.DestinationTypeAlias, Parameters.PropertyAlias);
        }



        protected virtual void DoRun(IEnumerable<string> sourceTypes, string destinationTypeAlias, string propertyAlias, string tabName = null)
        {
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

            sourceContentType.RemovePropertyType(oldPropertyAlias);
            ContentMigrationService.UpdateContentTypes(sourceContentType);            
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}