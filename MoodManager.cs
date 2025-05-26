using System;
using System.Collections.Generic;

namespace Chatbot_Luna
{
    
    public class MoodManager
    {
        // Dictionary mapping moods to keywords that indicate the mood in user input
        private static readonly Dictionary<string, string[]> moodKeywords = new Dictionary<string, string[]>
        {
            { "frustrated", new[] { "frustrated", "annoyed", "upset", "angry", "mad" } },
            { "confused", new[] { "confused", "lost", "puzzled", "unclear", "unsure" } },
            { "happy", new[] { "happy", "good", "great", "fantastic", "joyful" } },
            { "sad", new[] { "sad", "down", "unhappy", "depressed", "blue" } }
        };

        // Dictionary mapping moods to arrays of comforting responses
        private static readonly Dictionary<string, string[]> moodComfortResponses = new Dictionary<string, string[]>
        {
            { "frustrated", new[]
                {
                    "😓 I’m sorry you’re feeling frustrated. Cybersecurity can be tricky sometimes, but I’m here to help!",
                    "😣 Feeling frustrated is normal. Let’s work through this together.",
                    "😤 It’s okay to feel frustrated! Maybe I can help clear things up."
                }
            },
            { "confused", new[]
                {
                    "🤔 You sound a bit confused. Feel free to ask me anything about cybersecurity topics!",
                    "😕 No worries if things seem unclear. I’m here to explain!",
                    "💡 Confusion is a step toward understanding!"
                }
            },
            { "happy", new[]
                {
                    "😄 Great to hear you’re feeling good! Ready to learn more or have some fun?",
                    "🎉 Happy vibes make learning easier. Want a fun fact or tip?",
                    "😊 That’s wonderful! Cybersecurity knowledge is empowering, isn’t it?"
                }
            },
            { "sad", new[]
                {
                    "😔 Sorry you’re feeling down. Learning about staying safe online can give peace of mind.",
                    "🥺 Maybe a little cybersecurity tip can lighten the mood?",
                    "😢 Sad days happen. I’m here to help however I can."
                }
            }
        };

        // Dictionary mapping moods to arrays of helpful cybersecurity tips
        private static readonly Dictionary<string, string[]> moodTips = new Dictionary<string, string[]>
        {
            { "frustrated", new[]
                {
                    "💡 When frustrated, taking breaks between learning helps reset focus.",
                    "🧠 Break down tricky topics into smaller parts to avoid overwhelm.",
                    "🔥 Persistence pays off—even experts get frustrated sometimes!"
                }
            },
            { "confused", new[]
                {
                    "🔐 Use a password manager to simplify password security.",
                    "📨 Double-check email senders and links before clicking to avoid phishing.",
                    "🧭 Asking questions and researching helps clear confusion."
                }
            },
            { "happy", new[]
                {
                    "🛡️ Keep your software updated to stay secure!",
                    "👨‍👩‍👧 Share your cybersecurity knowledge with friends or family.",
                    "🧰 Explore new topics like firewall setup or encryption."
                }
            },
            { "sad", new[]
                {
                    "🔐 Set strong passwords to protect your online life.",
                    "🛡️ Enable two-factor authentication for extra security.",
                    "🧠 Knowing how to avoid scams can reduce worry."
                }
            }
        };

        
        private readonly Random rnd = new Random();

        /// <summary>
        /// Detects the user's mood based on input string by looking for mood keywords.
        /// </summary>
        /// <param name="input">User input text</param>
        /// <returns>Mood string if detected, otherwise null</returns>
        public string DetectMood(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

           
            input = input.ToLower();

           
            foreach (var mood in moodKeywords)
            {
                
                foreach (var keyword in mood.Value)
                {
                    if (input.Contains(keyword))
                        return mood.Key; 
                }
            }

            
            return null;
        }

        /// <summary>
        /// Gets a comforting response and tip based on the detected mood.
        /// </summary>
        /// <param name="mood">Detected mood string</param>
        /// <returns>Combined comforting message and tip</returns>
        public string GetMoodResponse(string mood)
        {
            if (string.IsNullOrEmpty(mood))
                return null;

            // Select a random comforting response for the mood
            string comfort = moodComfortResponses[mood][rnd.Next(moodComfortResponses[mood].Length)];
            
            string tip = moodTips[mood][rnd.Next(moodTips[mood].Length)];

            
            return $"{comfort}\nHere's a tip for you: {tip}";
        }

        /// <summary>
        /// Gets a comforting response and tip based on the detected mood,
        /// optionally customized with a topic mentioned by the user.
        /// </summary>
        /// <param name="mood">Detected mood string</param>
        /// <param name="topic">Optional topic provided by the user</param>
        /// <returns>Combined comforting message and tip with topic context</returns>
        public string GetMoodResponse(string mood, string topic)
        {
            if (string.IsNullOrEmpty(mood))
                return null;

            // Select a random comforting response for the mood
            string comfort = moodComfortResponses[mood][rnd.Next(moodComfortResponses[mood].Length)];
           
            string tip = moodTips[mood][rnd.Next(moodTips[mood].Length)];

            
            if (!string.IsNullOrEmpty(topic))
            {
                return $"{comfort}\nYou mentioned \"{topic}\" — here’s a tip that might help:\n{tip}";
            }

            
            return $"{comfort}\nHere's a tip for you: {tip}";
        }
    }
}
