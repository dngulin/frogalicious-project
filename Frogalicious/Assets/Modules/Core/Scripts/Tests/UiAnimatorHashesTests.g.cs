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
            Assert.That(UiAnimatorHashes.State.Appearing == Animator.StringToHash("Appearing"));
            Assert.That(UiAnimatorHashes.State.Appeared == Animator.StringToHash("Appeared"));
            Assert.That(UiAnimatorHashes.State.Disappeared == Animator.StringToHash("Disappeared"));
            Assert.That(UiAnimatorHashes.State.Disappearing == Animator.StringToHash("Disappearing"));
            Assert.That(UiAnimatorHashes.Parameter.IsVisible == Animator.StringToHash("IsVisible"));
        }
    }
}
