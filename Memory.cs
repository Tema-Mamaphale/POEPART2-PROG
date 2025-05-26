using System.Collections.Generic;

namespace Chatbot_Luna
{
    public static class Memory
    {
        public static string UserName { get; set; } = "";
        public static string CurrentTopic { get; set; } = "";
        public static string LastMood { get; set; } = "";
        public static string UserInterest { get; set; } = "";
        public static List<string> CoveredTopics { get; set; } = new List<string>();
        public static string PendingFollowup { get; set; } = "";

        // Tracks recent tips or topics to avoid repetition (capacity 3)
        public static List<string> SeenTips { get; set; } = new List<string>();

        // Added this dictionary to track recently used responses per topic
        public static Dictionary<string, List<int>> RecentResponses { get; set; } = new Dictionary<string, List<int>>();

        // Adds a tip or topic key if not already present, and keeps max 3
        public static void AddSeenTip(string tipKey)
        {
            if (!SeenTips.Contains(tipKey))
            {
                SeenTips.Add(tipKey);
                if (SeenTips.Count > 3)
                {
                    SeenTips.RemoveAt(0); 
                }
            }
        }
    }
}
