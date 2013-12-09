﻿using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    public static class AssertHelpers
    {
        public static void AssertBranch(DeployProjectBranch expected, DeployProjectBranch actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.BranchName, actual.BranchName);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        public static void AssertIsRecent(DateTime dateTime)
        {
            Assert.That(dateTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(2)));
        }

        public static void AssertDateEqual(DateTime expected, DateTime actual)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(2)));
        }

        public static void AssertBuild(DeployBuild expected, DeployBuild actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.ProjectComponentId, actual.ProjectComponentId);
            Assert.AreEqual(expected.ProjectComponentName, actual.ProjectComponentName);
            Assert.AreEqual(expected.ProjectBranchId, actual.ProjectBranchId);
            Assert.AreEqual(expected.ProjectBranchName, actual.ProjectBranchName);
            Assert.AreEqual(expected.FileId, actual.FileId);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        public static void AssertComponent(DeployComponent expected, DeployComponent actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ComponentName, actual.ComponentName);
            Assert.AreEqual(expected.IsolationType, actual.IsolationType);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
        }

        public static void AssertMachine(DeployMachine expectedMachine, DeployMachine actualMachine)
        {
            Assert.IsNotNull(actualMachine);
            Assert.AreEqual(expectedMachine.Id, actualMachine.Id);
            Assert.AreEqual(expectedMachine.ProjectId, actualMachine.ProjectId);
            Assert.AreEqual(expectedMachine.EnvironmentId, actualMachine.EnvironmentId);
            Assert.AreEqual(expectedMachine.EnvironmentName, actualMachine.EnvironmentName);
            Assert.AreEqual(expectedMachine.ParentId, actualMachine.ParentId);
            Assert.AreEqual(expectedMachine.CreatedByUserName, actualMachine.CreatedByUserName);
            AssertDateEqual(expectedMachine.CreatedDateTimeUtc, actualMachine.CreatedDateTimeUtc);
            Assert.AreEqual(expectedMachine.UpdatedByUserName, actualMachine.UpdatedByUserName);
            AssertDateEqual(expectedMachine.UpdatedDateTimeUtc, actualMachine.UpdatedDateTimeUtc);
            AssertDictionary(expectedMachine.ConfigurationValueList, actualMachine.ConfigurationValueList);
        }


        public static void AssertMachineList(List<DeployMachine> expectedList, List<DeployMachine> actualList)
        {
            foreach (var expectedMachine in expectedList)
            {
                var actualMachine = actualList.SingleOrDefault(i => i.MachineName == expectedMachine.MachineName);
                AssertMachine(expectedMachine, actualMachine);
            }
        }

        public static void AssertDictionary<T1, T2>(Dictionary<T1, T2> expected, Dictionary<T1, T2> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.AreEqual(expected.Count, actual.Count);
                foreach (var expectedKey in expected.Keys)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedKey));
                    Assert.AreEqual(expected[expectedKey], actual[expectedKey]);
                }
            }
        }
    }
}