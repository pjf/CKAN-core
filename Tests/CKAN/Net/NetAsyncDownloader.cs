﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tests;

namespace CKANTests
{
    /// <summary>
    /// Test the async downloader.
    /// </summary>
    
    [TestFixture]
    public class NetAsyncDownloader
    {

        private CKAN.Registry registry;
        private DisposableKSP ksp;


        [SetUp]
        public void Setup()
        {
            // Make sure curl is all set up.
            CKAN.Curl.Init();

            // Give us a registry to play with.
            ksp = new DisposableKSP();
            registry = ksp.KSP.Registry;

            registry.ClearAvailable();
            registry.ClearDlls();
            registry.Installed().Clear();

            // Make sure we have a registry we can use.
            CKAN.Repo.UpdateRegistry(TestData.TestKAN(), registry, ksp.KSP, new CKAN.NullUser());
        }

        [TearDown]
        public void TearDown()
        {
            CKAN.Curl.CleanUp();
            ksp.Dispose();
        }

        [Test]
        [Category("Online")]
        [Explicit]
        public void SingleDownload()
        {
            var async = new CKAN.NetAsyncDownloader(new CKAN.NullUser());

            // We know kOS is in the TestKAN data, and hosted in KS. Let's get it.

            var modules = new List<CKAN.CkanModule>();

            CKAN.CkanModule kOS = registry.LatestAvailable("kOS", null);
            Assert.IsNotNull(kOS);

            modules.Add(kOS);

            // Make sure we don't alread have kOS somehow.
            Assert.IsFalse(ksp.KSP.Cache.IsCached(kOS.download));

            // Download our module.
            async.DownloadModules(
                ksp.KSP.Cache,
                modules
            );

            // Assert that we have it, and it passes zip validation.
            Assert.IsTrue(ksp.KSP.Cache.IsCachedZip(kOS.download));
        }
    }
}

