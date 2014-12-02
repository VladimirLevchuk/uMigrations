using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI.WebControls;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using umbraco.NodeFactory;
using Vega.USiteBuilder;
using Vega.USiteBuilder.Repositories;
using Vega.USiteBuilder.Types;

namespace uMigrations.Tests.features.Framework
{
    public class DocumentRepository : Vega.USiteBuilder.Repositories.DocumentRepository
    {
        protected IContentService ContentService { get; private set; }

        public DocumentRepository(INodeRepository nodeRepository, IContentService contentService)
            : base(nodeRepository)
        {
            ContentService = contentService;
        }

        public override void Save<TContent>(TContent typedContent)
        {
            if (typedContent.ParentId != -1 && typedContent.ParentId != 0)
            {
                base.Save(typedContent);
                return;
            }

            IContent content;

            if (typedContent.Id == 0)
            {
                // create
                var contentTypeAlias = GetTypeAlias<TContent>();

                content = ContentService.CreateContent(typedContent.Name, -1, contentTypeAlias);

                typeof(DocumentTypeBase).GetProperty("Id").SetValue(typedContent, content.Id);
                // typedContent.SetNonPublicProperty(x => x.Id, content.Id);
            }
            else
            {
                content = ContentService.GetById(typedContent.Id);
            }

            // update part
            var properties = GetPropertyValues(typedContent);

            SavePropertyValues(content, properties);

            ContentService.Save(content);
        }

        private void SavePropertyValues(IContent content, Dictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                content.Properties[property.Key].Value = property.Value;
            }
        }

        private Dictionary<string, object> GetPropertyValues<T>(T typedContent)
            where T: DocumentTypeBase
        {
            var result = new Dictionary<string, object>();

            var properties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StoreDocumentTypeProperties(result, properties, typedContent);

            return result;
        }

        private void StoreDocumentTypeProperties(Dictionary<string, object> storage, IEnumerable<PropertyInfo> properties, object typedContent)
        {
            foreach (var propInfo in properties)
            {
                // process mixin properties
                var mixinAttribute = SiteBuilderTools.GetAttribute<MixinPropertyAttribute>(propInfo);
                if (mixinAttribute != null)
                {
                    var mixin = propInfo.GetValue(typedContent, null);
                    if (mixin != null)
                    {
                        var mixinType = mixinAttribute.GetMixinType(propInfo);
                        var mixinProperties = mixinType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        StoreDocumentTypeProperties(storage, mixinProperties, mixin);
                    }

                    continue;
                }

                // process document type properties
                var propAttr = SiteBuilderTools.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a Document Type
                }

                string propertyName;
                string propertyAlias;
                SiteBuilderTools.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                var propertyConvertors = typeof (ContentHelper).GetStaticNonPublicProperty<Dictionary<Type, ICustomTypeConvertor>>("PropertyConvertors");

                if (propertyConvertors.ContainsKey(propInfo.PropertyType))
                {
                    storage[propertyAlias] = propertyConvertors[propInfo.PropertyType].ConvertValueWhenWrite(propInfo.GetValue(typedContent, null));
                }
                else
                {
                    storage[propertyAlias] = propInfo.GetValue(typedContent, null);
                }
            }
        }

        protected string GetTypeAlias<TContent>()
        {
            return SiteBuilderTools.GetDocumentTypeAlias(typeof (TContent));
        }
    }

    public static class MemberAccessExpressionExtensions
    {
        public static string GetParameterNameFromExpression<TObject, TParameter>(
            this Expression<Func<TObject, TParameter>> memberAccessExpression)
        {
            if (memberAccessExpression == null)
            {
                throw new ArgumentNullException("memberAccessExpression");
            }

            var memberExpression = memberAccessExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Expression is not a member access expression", "memberAccessExpression");
            }

            return memberExpression.Member.Name;
        }

        public static void SetNonPublicProperty<TObject, TParameter>(this TObject @object, Expression<Func<TObject, TParameter>> memberAccessExpression, TParameter value)
        {
            var name = memberAccessExpression.GetParameterNameFromExpression();

            var property = @object.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.SetProperty);

            property.SetValue(@object, value);
        }

        public static TResult GetStaticNonPublicProperty<TResult>(this Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);

            return (TResult) property.GetValue(null);
        }
    }

    internal static class SiteBuilderTools
    {
        internal static DocumentTypeAttribute GetDocumentTypeAttribute(Type typeDocType)
        {
            var retVal = GetAttribute<DocumentTypeAttribute>(typeDocType);

            if (retVal == null)
            {
                retVal = CreateDefaultDocumentTypeAttribute(typeDocType);
            }

            return retVal;
        }

        private static DocumentTypeAttribute CreateDefaultDocumentTypeAttribute(Type typeDocType)
        {
            var retVal = new DocumentTypeAttribute();

            retVal.Name = typeDocType.Name;
            retVal.IconUrl = DocumentTypeDefaultValues.IconUrl;
            retVal.Thumbnail = DocumentTypeDefaultValues.Thumbnail;

            return retVal;
        }

        public static T GetAttribute<T>(Type type)
        {
            T retVal;

            object[] attributes = type.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                retVal = (T)attributes[0];
            }
            else
            {
                retVal = default(T);
            }

            return retVal;
        }

        public static T GetAttribute<T>(PropertyInfo propertyInfo)
        {
            T retVal;

            object[] attributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                retVal = (T)attributes[0];
            }
            else
            {
                retVal = default(T);
            }

            return retVal;
        }


        // from DocumentTypeManager
        public static string GetDocumentTypeAlias(Type typeDocType)
        {
            string alias;
            bool aliasUsed = false;

            DocumentTypeAttribute docTypeAttr = GetDocumentTypeAttribute(typeDocType);

            if (!String.IsNullOrEmpty(docTypeAttr.Alias))
            {
                alias = docTypeAttr.Alias;
                aliasUsed = true;
            }
            else
            {
                alias = typeDocType.Name;
            }

            if (alias.ToLower() != alias.ToSafeAlias().ToLower())
                throw new ArgumentException(string.Format("The {0} '{1}', for the document type '{2}', is invalid.", (aliasUsed ? "alias" : "name"), alias, typeDocType.Name), "Alias");

            return alias;
        }

        public static void ReadPropertyNameAndAlias(PropertyInfo propInfo, DocumentTypePropertyAttribute propAttr,
            out string name, out string alias)
        {


            // set name
            name = string.IsNullOrEmpty(propAttr.Name) ? propInfo.Name : propAttr.Name;

            // set a default alias
            alias = propInfo.Name.Substring(0, 1).ToLower();

            // if an alias has been set, use that one explicitly
            if (!String.IsNullOrEmpty(propAttr.Alias))
            {
                alias = propAttr.Alias;

                // otherwise
            }
            else
            {

                // create the alias from the name 
                if (propInfo.Name.Length > 1)
                {
                    alias += propInfo.Name.Substring(1, propInfo.Name.Length - 1);
                }

                // This is required because it seems that Umbraco has a bug when property type alias is called pageName.
                if (alias == "pageName")
                {
                    alias += "_";
                }
            }
        }


        //private static void PopulateUmbracoValues<T>(T typedPage, IContent content)
        //    where T : DocumentTypeBase
        //{
        //    typedPage.Id = content.Id;
        //    typedPage.Name = content.Name;
        //    typedPage.ParentId = parentId;
        //    typedPage.CreateDate = node.CreateDate;
        //    typedPage.UpdateDate = node.UpdateDate;
        //    typedPage.CreatorId = node.CreatorID;
        //    typedPage.CreatorName = node.CreatorName;
        //    typedPage.NiceUrl = node.NiceUrl;
        //    typedPage.NodeTypeAlias = node.NodeTypeAlias;
        //    typedPage.Path = node.Path;
        //    typedPage.SortOrder = node.SortOrder;
        //    typedPage.Template = node.template;
        //    typedPage.Url = node.Url;
        //    typedPage.WriterID = node.WriterID;
        //    typedPage.WriterName = node.WriterName;
        //    typedPage.Version = node.Version;
        //    typedPage.Source = node;
        //}
    }
}