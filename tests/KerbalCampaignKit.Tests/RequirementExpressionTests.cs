using KerbalDialogueKit.Flags;
using KerbalCampaignKit.Triggers;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class RequirementExpressionTests
    {
        [Fact]
        public void NullExpression_EvaluatesTrue()
        {
            var flags = new FlagStore();
            Assert.True(RequirementExpression.Evaluate(null, flags));
            Assert.True(RequirementExpression.Evaluate("", flags));
        }

        [Fact]
        public void SimpleEquality()
        {
            var flags = new FlagStore();
            flags.Set("chapter", "3");
            Assert.True(RequirementExpression.Evaluate("chapter == 3", flags));
            Assert.False(RequirementExpression.Evaluate("chapter == 4", flags));
        }

        [Fact]
        public void AndOr()
        {
            var flags = new FlagStore();
            flags.Set("chapter", "3");
            flags.Set("mood", "happy");
            Assert.True(RequirementExpression.Evaluate("chapter == 3 && mood == happy", flags));
            Assert.False(RequirementExpression.Evaluate("chapter == 3 && mood == sad", flags));
            Assert.True(RequirementExpression.Evaluate("chapter == 99 || mood == happy", flags));
        }

        [Fact]
        public void InvalidExpression_EvaluatesFalse_NoCrash()
        {
            var flags = new FlagStore();
            Assert.False(RequirementExpression.Evaluate("not a real expression", flags));
        }
    }
}
