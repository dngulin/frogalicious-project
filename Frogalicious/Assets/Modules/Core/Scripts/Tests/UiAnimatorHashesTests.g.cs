using NUnit.Framework;
using UnityEngine;
using Frog.Core.Ui;

namespace Frog.Core.Tests
{
    [TestFixture]
    public class UiAnimatorHashesTests
    {
        [Test]
        public void CompareHashes()
        {
            Assert.That(UiAnimatorHashes.StateHashes.Appearing == Animator.StringToHash("Appearing"));
            Assert.That(UiAnimatorHashes.StateHashes.Appeared == Animator.StringToHash("Appeared"));
            Assert.That(UiAnimatorHashes.StateHashes.Disappeared == Animator.StringToHash("Disappeared"));
            Assert.That(UiAnimatorHashes.StateHashes.Disappearing == Animator.StringToHash("Disappearing"));
            Assert.That(UiAnimatorHashes.ParamHashes.IsVisible == Animator.StringToHash("IsVisible"));
        }
    }
}
