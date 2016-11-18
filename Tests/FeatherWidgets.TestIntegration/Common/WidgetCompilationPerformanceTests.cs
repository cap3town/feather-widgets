﻿using System;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using FeatherWidgets.TestUtilities.CommonOperations;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.ContentBlock.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUtilities.Modules.Diagnostics;
using Telerik.Sitefinity.Web;

namespace FeatherWidgets.TestIntegration.Common
{
    /// <summary>
    /// This class contains tests for the performance method region and tracking razor view compilations.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.Common)]
    [Description("This class contains tests for the performance method region and tracking razor view compilations.")]
    public class WidgetCompilationPerformanceTests : ProfilingTestBase
    {
        #region Tests

        /// <summary>
        /// Verifies that when widget template is edited and page is requested the execution and the compilation of the MVC widgets is logged correctly.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Sitefinity.TestUtilities.CommonOperations.WidgetOperations.AddContentBlockToPage(System.Guid,System.String,System.String,System.String)"), Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Verifies that when widget template is edited and page is requested the execution and the compilation of the MVC widgets is logged correctly.")]
        public void ModifiedView_RequestPage_ShouldLogCompilation()
        {
            string viewFileName = "Default.cshtml";
            string widgetName = "ContentBlock";

            var widgetText = @"@Html.Raw(Model.Content)";
            var widgetTextEdited = @"edited @Html.Raw(Model.Content)";
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(ResourcePackages.Bootstrap, widgetName, viewFileName);

            PageNode pageNode = null;
            try
            {
                this.EnableProfiler("HttpRequestsProfiler");
                this.EnableProfiler("WidgetExecutionsProfiler");
                this.EnableProfiler("RazorViewCompilationsProfiler");

                Guid templateId = Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
                var pageId = ServerOperations.Pages().CreatePage("TestPage1", templateId);
                var pageNodeId = ServerOperations.Pages().GetPageNodeId(pageId);
                var pageManager = Telerik.Sitefinity.Modules.Pages.PageManager.GetManager();
                pageNode = pageManager.GetPageNode(pageNodeId);
                var fullPageUrl = RouteHelper.GetAbsoluteUrl(pageNode.GetUrl());

                int widgetCount = 3;
                for (var i = 0; i < widgetCount; i++)
                    ServerOperationsFeather.Pages().AddContentBlockWidgetToPage(pageNodeId, "ContentBlock", "Contentplaceholder1");

                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();
                this.ClearData();

                var viewPath = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend.ContentBlock/Mvc/Views/ContentBlock/Default.cshtml";
                var fullViewPath = string.Concat(viewPath, "#Bootstrap.cshtml");

                this.InvalidateAspNetRazorViewCache(fullViewPath);
                this.WaitForAspNetCacheToBeInvalidated(fullViewPath);

                // Request page
                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();

                this.AssertWidgetExecutionCount(widgetCount);
                this.AssertViewCompilationCount(1);

                // Assert data
                var rootOperationId = this.GetRequestLogRootOperationId(fullPageUrl);

                var widgetCompilationText = "Compile view \"Default.cshtml#Bootstrap.cshtml\" of controller \"" + typeof(ContentBlockController).FullName + "\"";
                this.AssertViewCompilationParams(rootOperationId, viewPath, widgetCompilationText);

                this.ClearData();

                // Request page again
                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();

                // Assert new data
                this.AssertWidgetExecutionCount(widgetCount);
                this.AssertViewCompilationCount(0);
            }
            finally
            {
                this.DeletePages(pageNode);
            }
        }

        /// <summary>
        /// Verifies that when widget template file is overwritten and page is requested the execution and the compilation of the MVC widget is logged.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Verifies that when widget template file is overwritten and page is requested the execution and the compilation of the MVC widget is logged.")]
        public void OverwrittenView_RequestPage_ShouldLogRazorViewCompilation()
        {
            string viewFileName = "Default.cshtml";
            string widgetName = "ContentBlock";

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(ResourcePackages.Bootstrap, widgetName, viewFileName);

            PageNode pageNode = null;
            try
            {
                this.EnableProfiler("HttpRequestsProfiler");
                this.EnableProfiler("WidgetExecutionsProfiler");
                this.EnableProfiler("RazorViewCompilationsProfiler");

                Guid templateId = Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
                var pageId = ServerOperations.Pages().CreatePage("TestPage1", templateId);
                var pageNodeId = ServerOperations.Pages().GetPageNodeId(pageId);
                var pageManager = Telerik.Sitefinity.Modules.Pages.PageManager.GetManager();
                pageNode = pageManager.GetPageNode(pageNodeId);
                var fullPageUrl = RouteHelper.GetAbsoluteUrl(pageNode.GetUrl());

                int widgetCount = 3;
                for (var i = 0; i < widgetCount; i++)
                    ServerOperationsFeather.Pages().AddContentBlockWidgetToPage(pageNodeId, "ContentBlock", "Contentplaceholder1");

                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();
                this.ClearData();

                var viewPath = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend.ContentBlock/Mvc/Views/ContentBlock/Default.cshtml";
                var fullViewPath = string.Concat(viewPath, "#Bootstrap.cshtml");

                this.InvalidateAspNetRazorViewCache(fullViewPath);
                this.WaitForAspNetCacheToBeInvalidated(fullViewPath);

                // request page
                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();

                this.AssertWidgetExecutionCount(widgetCount);
                this.AssertViewCompilationCount(1);

                // Assert data
                var rootOperationId = this.GetRequestLogRootOperationId(fullPageUrl);

                var widgetCompilationText = "Compile view \"Default.cshtml#Bootstrap.cshtml\" of controller \"" + typeof(ContentBlockController).FullName + "\"";
                this.AssertViewCompilationParams(rootOperationId, viewPath, widgetCompilationText);

                this.ClearData();

                // Request page again
                this.ExecuteAuthenticatedRequest(fullPageUrl);
                this.FlushData();

                // Assert new data
                this.AssertWidgetExecutionCount(widgetCount);
                this.AssertViewCompilationCount(0);
            }
            finally
            {
                this.DeletePages(pageNode);
            }
        }

        #endregion

        #region Private Methods

        private void OvewriteFile(string filePath)
        {
            string contents = File.ReadAllText(filePath);
            contents += " ";

            File.Delete(filePath);
            File.WriteAllText(filePath, contents);
        }

        private void InvalidateAspNetRazorViewCache(string virtualPath)
        {
            if (BuildManager.GetCachedBuildDependencySet(null, virtualPath) == null)
                return;

            var compiledType = BuildManager.GetCompiledType(virtualPath);
            var compiledTypeAssemblyName = string.Concat(compiledType.Assembly.GetName().Name, ".dll");

            var webCompilationAssembly = typeof(BuildManager).Assembly;
            var memoryCacheType = webCompilationAssembly.GetType("System.Web.Compilation.MemoryBuildResultCache");
            var diskCacheType = webCompilationAssembly.GetType("System.Web.Compilation.DiskBuildResultCache");

            var buildManagerInstance = typeof(BuildManager).GetField("_theBuildManager", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var caches = (object[])typeof(BuildManager).GetField("_caches", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(buildManagerInstance);

            foreach (var cache in caches)
            {
                var cacheType = cache.GetType();
                if (memoryCacheType.IsAssignableFrom(cacheType))
                {
                    this.InvalidateMemoryCache(cache, virtualPath);
                }
                else if (diskCacheType.IsAssignableFrom(cacheType))
                {
                    this.InvalidateDiskCache(cache, compiledTypeAssemblyName);
                }
            }
        }

        private void InvalidateMemoryCache(object cache, string virtualPath)
        {
            var webCompilationAssembly = typeof(BuildManager).Assembly;
            var virtualPathType = webCompilationAssembly.GetType("System.Web.VirtualPath");
            var virtualPathCreateMethod = virtualPathType.GetMethod("Create", new Type[] { typeof(string) });
            var virtualPathObject = virtualPathCreateMethod.Invoke(null, new[] { virtualPath });

            var getCacheKeyMethod = typeof(BuildManager).GetMethod("GetCacheKeyFromVirtualPath", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { virtualPathType }, new ParameterModifier[0]);
            var cacheKey = getCacheKeyMethod.Invoke(null, new[] { virtualPathObject });
            var getMemoryCacheKey = cache.GetType().GetMethod("GetMemoryCacheKey", BindingFlags.Static | BindingFlags.NonPublic);
            var key = getMemoryCacheKey.Invoke(null, new[] { cacheKey });
            var internalCache = cache.GetType().GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cache);
            var removeBuildResult = webCompilationAssembly.GetType("System.Web.Caching.CacheInternal").GetMethod("Remove", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(string) }, new ParameterModifier[] { });
            removeBuildResult.Invoke(internalCache, new[] { key });
        }

        private void InvalidateDiskCache(object cache, string assemblyName)
        {
            var webCompilationAssembly = typeof(BuildManager).Assembly;
            var removeAssemblyMethod = cache.GetType().GetMethod("RemoveAssemblyAndRelatedFiles", BindingFlags.Instance | BindingFlags.NonPublic);
            removeAssemblyMethod.Invoke(cache, new[] { assemblyName });

            var removeTempFilesMethod = cache.GetType().GetMethod("RemoveOldTempFiles", BindingFlags.Instance | BindingFlags.NonPublic);
            removeTempFilesMethod.Invoke(cache, new object[] { });
        }

        #endregion

        #region Fields and Constants

        private struct ResourcePackages
        {
            public const string Bootstrap = "Bootstrap";
        }

        private const string WidgetViewPathFormat = "";
        private const string PageTemplateName = "Bootstrap.default";

        #endregion
    }
}