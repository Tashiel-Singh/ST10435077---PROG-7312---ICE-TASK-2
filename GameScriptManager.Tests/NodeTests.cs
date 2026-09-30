using GameScriptManager.Models;
using Xunit;

namespace GameScriptManager.Tests;

public class NodeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var node = new Node(5, "In the fortress...");

        Assert.Equal(5, node.StoryNumber);
        Assert.Equal("In the fortress...", node.StoryText);
        Assert.Null(node.Next);
        Assert.Null(node.Previous);
    }

    [Fact]
    public void Constructor_HandlesNullTextGracefully()
    {
        var node = new Node(1, null!);

        Assert.Equal(string.Empty, node.StoryText);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var node = new Node(42, "Cyber battle begins.");

        Assert.Equal("[42] Cyber battle begins.", node.ToString());
    }

    [Fact]
    public void Pointers_CanBeLinkedBidirectionally()
    {
        var first = new Node(1, "Beginning");
        var second = new Node(2, "Middle");

        first.Next = second;
        second.Previous = first;

        Assert.Same(second, first.Next);
        Assert.Same(first, second.Previous);
    }
}
