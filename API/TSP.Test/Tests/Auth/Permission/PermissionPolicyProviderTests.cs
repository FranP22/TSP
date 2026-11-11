using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using TSP.Common.Enum;
using TSP.Controller.Auth;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Auth.Permission
{
    [TestFixture]
    public class PermissionPolicyProviderTests
    {
        private PermissionPolicyProvider _provider;

        private static IEnumerable<TestCaseData> PermissionCases()
        {
            foreach (var permission in Enum.GetValues<RolePermission>())
            {
                yield return new TestCaseData(permission);
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            var options = Options.Create(new AuthorizationOptions());

            _provider = new PermissionPolicyProvider(options);
        }

        [TestCaseSource(nameof(PermissionCases))]
        [Test]
        public async Task GetPolicyAsync_WithValidPermission_ReturnsPolicy(RolePermission permission)
        {
            var policyName = $"Permission:{permission}";

            var policy = await _provider.GetPolicyAsync(policyName);

            Assert.That(policy, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(policy!.Requirements, Has.Some.InstanceOf<DenyAnonymousAuthorizationRequirement>());

                Assert.That(policy.Requirements.OfType<PermissionRequirement>().Count(), Is.EqualTo(1));

                Assert.That(policy.Requirements.OfType<PermissionRequirement>().Single().Permission, Is.EqualTo(permission));
            });
        }

        [TestCase("Permission:InvalidPermission")]
        [TestCase("Permission:")]
        [TestCase("Permission:123456")]
        public async Task GetPolicyAsync_WithInvalidPermission_ReturnsNull(string policyName)
        {
            var policy = await _provider.GetPolicyAsync(policyName);

            Assert.That(policy, Is.Null);
        }

        [Test]
        public async Task GetPolicyAsync_WithUnknownPolicy_ReturnsNull()
        {
            var policy = await _provider.GetPolicyAsync("UnknownPolicy");

            Assert.That(policy, Is.Null);
        }
    }
}
