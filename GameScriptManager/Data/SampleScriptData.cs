using GameScriptManager.Models;

namespace GameScriptManager.Data;

/// <summary>
/// Provides the unordered sample script data as specified in the assignment specification.
/// </summary>
public static class SampleScriptData
{
    /// <summary>
    /// Gets the list of 12 unordered script paragraphs in the exact original sequence from the assignment sheet.
    /// </summary>
    public static readonly IReadOnlyList<(int Number, string Text)> RawUnorderedData = new List<(int, string)>
    {
        (3, "With every line of code mastered, Alex gains experience points, leveling up and unlocking new abilities like Debugging Dash and Algorithmic Aura."),
        (12, "The tale of Alex, the IT student-turned-digital-legend, is forever etched in the annals of Cybersphere, inspiring aspiring programmers to pursue their dreams."),
        (4, "The Firewall Fortress looms ahead, its defenses formidable, but Alex's mastery of cybersecurity allows them to breach the walls with a series of perfectly timed hacks."),
        (2, "Armed with a trusty keyboard and a digital sword, Alex enters the Coding Caverns, where bugs and glitches guard the treasures of programming wisdom."),
        (1, "In the virtual realm of Cybersphere, our hero, Alex, a determined IT student, embarks on an epic quest for knowledge."),
        (7, "Along the journey, Alex forges alliances with NPC coders, forming a guild known as \"Syntax Sentinels,\" united by their dedication to digital mastery."),
        (10, "Victory is hard-won, but Alex's leadership and IT prowess lead to the defeat of the Malware Marauders, restoring peace to Cybersphere."),
        (11, "Celebrated as a digital hero, Alex stands at the forefront of innovation, using the knowledge gained to create groundbreaking applications that shape the future of technology."),
        (9, "In a climactic battle, Alex and the Syntax Sentinels engage in a virtual war, using complex algorithms and strategic thinking to outwit the Malware Marauders' schemes."),
        (5, "Inside the fortress, a virtual reality riddle awaits – a puzzle that can only be solved by applying principles of encryption and decryption learned during countless late-night study sessions."),
        (6, "Emerging victorious, Alex discovers the hidden Repository of the Ancients, a collection of long-lost IT texts rumored to contain the ultimate programming language."),
        (8, "The guild faces its nemesis in the form of the Malware Marauders, a rival group aiming to spread chaos and disrupt the digital realm.")
    };

    /// <summary>
    /// Populates and returns a new <see cref="StoryLinkedList"/> with the unordered sample script data.
    /// Fulfills Requirement 2: "Populate the Linked List".
    /// </summary>
    /// <returns>A populated <see cref="StoryLinkedList"/> in disordered sequence.</returns>
    public static StoryLinkedList PopulateSampleScript()
    {
        var list = new StoryLinkedList();
        foreach (var (number, text) in RawUnorderedData)
        {
            list.Add(number, text);
        }
        return list;
    }
}
