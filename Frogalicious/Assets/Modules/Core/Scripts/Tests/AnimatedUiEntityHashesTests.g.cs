using NUnit.Framework;
using UnityEngine;
using Frog.Core.Ui;

namespace Frog.Core.Tests
{
    [TestFixture]
    public class AnimatedUiEntityHashesTests
    {
        [Test]
        public void CompareHashes()
        {
            Assert.That(AnimatedUiEntityHashes.State.Appearing == Animator.StringToHash("Appearing"));
            Assert.That(AnimatedUiEntityHashes.State.Appeared == Animator.StringToHash("Appeared"));
            Assert.That(AnimatedUiEntityHashes.State.Disappeared == Animator.StringToHash("Disappeared"));
            Assert.That(AnimatedUiEntityHashes.State.Disappearing == Animator.StringToHash("Disappearing"));
            Assert.That(AnimatedUiEntityHashes.Parameter.IsVisible == Animator.StringToHash("IsVisible"));
        }
    }
}
