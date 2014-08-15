﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace FeatherWidgets.TestUI.Arrangements
{
    /// <summary>
    /// Sample arrangement that Creates and deletes a page.
    /// </summary>
    public class ContentBlockWidgetOnPage : ITestArrangement
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            ServerOperations.Pages().CreatePage(PageName);

        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "ContentBlock";
    }
}
