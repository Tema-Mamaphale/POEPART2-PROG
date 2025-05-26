using System;
using System.Collections.Generic;
using System.Linq;

namespace LunaCyberSecurityChatbot
{
    public class LunaBot
    {
        private readonly Dictionary<string, List<string>> topicTips = new Dictionary<string, List<string>>()
        {
            {"phishing", new List<string> {
                "🚫 Never click suspicious links in emails claiming urgency.",
                "🔍 Hover over links to check their destination before clicking.",
                "📢 Report phishing attempts to your IT or email provider."
            }},
            {"passwords", new List<string> {
                "🔐 Use a password manager to create and store strong passwords.",
                "🚫 Avoid reusing the same password across multiple sites.",
                "💡 Include uppercase, lowercase, numbers, and symbols."
            }},
            {"malware", new List<string> {
                "🛡️ Keep antivirus software updated.",
                "⚠️ Avoid downloading files from untrusted sources.",
                "🔒 Don't disable security features on your device."
            }},
            {"vpn", new List<string> {
                "🕵️ Use a VPN when on public Wi-Fi to encrypt your traffic.",
                "✅ Choose a reputable, no-log VPN provider.",
                "🔄 Enable VPN auto-connect on your devices."
            }},
            {"ransomware", new List<string> {
                "💾 Regularly back up your data to offline or cloud storage.",
                "📎 Don't open unexpected attachments from unknown senders.",
                "🔄 Keep your system and antivirus software updated."
            }},
            {"firewalls", new List<string> {
                "🔥 Enable firewalls on all your network devices.",
                "🛡️ Use both hardware and software firewalls for layered defense.",
                "📝 Regularly review and update firewall rules."
            }},
            {"social engineering", new List<string> {
                "🤨 Be skeptical of urgent requests for personal info.",
                "🕵️ Verify identities before sharing sensitive information.",
                "📚 Educate staff about manipulation techniques like pretexting."
            }},
            {"data breaches", new List<string> {
                "🔑 Use strong, unique passwords for every account.",
                "👀 Monitor your accounts for unusual activity.",
                "🔄 Change passwords immediately if a breach occurs."
            }},
            {"encryption", new List<string> {
                "🔐 Encrypt sensitive files before sending or storing them.",
                "💻 Use full disk encryption for laptops and mobile devices.",
                "🚫 Avoid transmitting sensitive data over unencrypted connections."
            }},
            {"2fa", new List<string> {
                "🔒 Enable two-factor authentication wherever possible.",
                "📱 Use app-based 2FA (like Authenticator) instead of SMS when available.",
                "🗂️ Keep backup codes secure in case you lose access to your 2FA device."
            }},
            {"updates", new List<string> {
                "🔄 Enable automatic updates on all software and devices.",
                "⚙️ Install patches as soon as they’re released.",
                "⚠️ Outdated software can become a security risk."
            }},
            {"public wi-fi", new List<string> {
                "🚫 Avoid accessing banking or personal accounts on public Wi-Fi.",
                "🕵️ Use a VPN to protect your data on public networks.",
                "⚙️ Disable auto-connect to public hotspots."
            }}
        };

        // Track which tips have been used per topic to avoid repetition until all are used.
        private readonly Dictionary<string, HashSet<string>> usedTopicTips = new Dictionary<string, HashSet<string>>();

        // List of topics the user has asked about in this conversation.
        private readonly List<string> conversationTopics = new List<string>();
        private readonly HashSet<string> acknowledgedMoods = new HashSet<string>();
        private readonly Random random = new Random();

        private string userName = "";
        private string userInterest = "";
        private string lastTopic = null;
        private string lastMood = null;
        private DateTime lastMoodTime = DateTime.MinValue;

        // Maps synonyms to primary topic keys to improve user input understanding
        private readonly Dictionary<string, string> topicSynonyms = new Dictionary<string, string>()
        {
            {"online safety", "vpn"},
            {"internet privacy", "vpn"},
            {"secure browsing", "vpn"},
            {"secure passwords", "passwords"},
            {"security updates", "updates"},
            {"multifactor", "2fa"},
            {"identity verification", "2fa"},
            {"online scams", "phishing"},
            {"hacking", "malware"},
            {"personal data", "data breaches"}
        };

        // Returns a time-aware greeting when the user first starts interaction.
        public string GreetUser()
        {
            int hour = DateTime.Now.Hour;
            string greeting = hour < 12 ? "🌅 Good morning" :
                              hour < 18 ? "☀️ Good afternoon" :
                                          "🌙 Good evening";

            return $"{greeting}! I'm Luna, your cybersecurity assistant 🤖. What's your name?";
        }

        public void RememberUserName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                userName = name.Trim();
        }

        // Normalize and map interest synonyms to main topics.
        public void RememberUserInterest(string interest)
        {
            if (!string.IsNullOrWhiteSpace(interest))
            {
                string cleaned = interest.Trim().ToLower();
                if (topicSynonyms.ContainsKey(cleaned))
                    userInterest = topicSynonyms[cleaned];
                else if (topicTips.ContainsKey(cleaned))
                    userInterest = cleaned;
            }
        }

        public string WelcomeUser()
        {
            if (string.IsNullOrEmpty(userName))
                return "Hello! What cybersecurity topic interests you today? 🔐";

            if (!string.IsNullOrEmpty(userInterest))
                return $"Nice to meet you, {userName}! I'm excited to help you learn about {userInterest} 🌟.";

            return $"Nice to meet you, {userName}! What cybersecurity topic interests you? 🤔";
        }

        public string GetDailyTip()
        {
            var allTips = topicTips.Values.SelectMany(list => list).ToList();
            var usedTips = usedTopicTips.Values.SelectMany(tips => tips).ToList();
            var availableTips = allTips.Where(t => !usedTips.Contains(t)).ToList();

            if (availableTips.Count == 0)
                availableTips = allTips;

            string selectedTip = availableTips[random.Next(availableTips.Count)];
            return "💡 Tip of the Day: " + selectedTip;
        }

        public string ProcessInput(string input, string detectedMood = null, string detectedTopic = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "How can I assist you with cybersecurity today? 🔐";

            string userInput = input.Trim().ToLower();

            // Follow-up detection comes first
            var followUpPhrases = new List<string>
            {
                "yes", "tell me more", "more info", "more", "go on", "expand", "help me understand", "another tip", "what else"
            };

            if (followUpPhrases.Any(p => userInput.Contains(p)))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return RespondWithFollowUp(lastTopic);
                return "🧠 I'd love to give more tips! Can you remind me which topic you're asking about?";
            }

            // Mood handling with cooldown
            if (!string.IsNullOrEmpty(detectedMood))
            {
                if (detectedMood == lastMood && (DateTime.Now - lastMoodTime).TotalSeconds < 30)
                    return "I hear you. If you'd like, I can share some tips or change the topic. 😊";

                lastMood = detectedMood;
                lastMoodTime = DateTime.Now;
                acknowledgedMoods.Add(detectedMood);

                return RespondToMoodBrief(detectedMood);
            }

            // Topic detection
            if (!string.IsNullOrEmpty(detectedTopic))
            {
                lastTopic = detectedTopic;
                if (!conversationTopics.Contains(detectedTopic))
                    conversationTopics.Add(detectedTopic);
                return RespondToTopic(detectedTopic);
            }

            // Commands
            if (userInput.Contains("help"))
                return ShowHelpMenu();

            if (userInput.Contains("tip"))
                return GetDailyTip();

            if (userInput.Contains("bye") || userInput.Contains("exit") || userInput.Contains("quit"))
                return GetGoodbyeMessage();

            return "😕 Sorry, I didn't quite catch that. Could you please specify a cybersecurity topic or ask for a tip?";
        }

        private string RespondToMoodBrief(string mood)
        {
            switch (mood)
            {
                case "happy":
                    return "😊 That's great to hear! Ready to dive into more cybersecurity tips?";
                case "frustrated":
                case "annoyed":
                    return "😟 I understand it can be frustrating. I'm here to help — just let me know what you'd like to do next.";
                case "confused":
                    return "🤔 It's okay to be confused. Want help understanding a specific topic?";
                case "anxious":
                    return "😰 Don’t worry — cybersecurity can be tricky. I'm here to make it easier.";
                case "sad":
                    return "😢 Sorry you're feeling down. Let's make cybersecurity learning a bit more fun.";
                default:
                    return "🤖 I'm here to help however I can.";
            }
        }

        private string RespondToTopic(string topic)
        {
            string def = GetDefinition(topic);
            string tip = GetRandomTip(topic);
            string example = GetExample(topic);

            return $"🔎 Let's talk about {topic}:\n\n{def}\n\n💡 Tip: {tip}\n\n📌 Example: {example}\n\nWould you like another tip or a different topic? 😊";
        }

        // Provides another tip for the current topic or asks for clarification.

        private string RespondWithFollowUp(string topic)
        {
            if (topicTips.ContainsKey(topic))
                return $"🔄 Here's another tip on {topic}:\n💡 {GetRandomTip(topic)}\n\nNeed more help or want to switch topics?";
            return "🤔 I'm not sure what to follow up on. Could you tell me which topic you're interested in?";
        }

        // Static definitions for known topics
        private string GetDefinition(string topic)
        {
            var definitions = new Dictionary<string, string>
            {
                {"phishing", "Phishing tricks you into revealing info by posing as a trusted source."},
                {"passwords", "Passwords protect your accounts. Strong passwords reduce the risk of compromise."},
                {"malware", "Malware is software designed to harm, exploit, or damage devices."},
                {"vpn", "A VPN encrypts your internet connection to protect your privacy."},
                {"ransomware", "Ransomware locks your files and demands a ransom to unlock them."},
                {"firewalls", "Firewalls monitor and filter network traffic to prevent threats."},
                {"social engineering", "Social engineering manipulates people into giving away sensitive data."},
                {"data breaches", "A data breach exposes confidential data to unauthorized access."},
                {"encryption", "Encryption converts data into unreadable code to protect it from unauthorized users."},
                {"2fa", "Two-factor authentication adds an extra verification step to logins."},
                {"updates", "Updates fix security flaws and improve system protection."},
                {"public wi-fi", "Public Wi-Fi is risky and can expose your data without protection like a VPN."}
            };

            return definitions.ContainsKey(topic) ? definitions[topic] : "I don't have a definition for that topic yet.";
        }

        private string GetExample(string topic)
        {
            var examples = new Dictionary<string, string>
            {
                {"phishing", "An email from 'your bank' asking to verify login details."},
                {"passwords", "Using 'MyD0g!sC00l2024' instead of 'password123'."},
                {"malware", "Downloading an attachment from an unknown sender that installs spyware."},
                {"vpn", "Using a VPN in a coffee shop to secure your browsing."},
                {"ransomware", "Getting a popup asking for payment to unlock your files."},
                {"firewalls", "Your router blocking suspicious network activity."},
                {"social engineering", "A call pretending to be IT asking for your credentials."},
                {"data breaches", "Hackers leaking millions of passwords from a website."},
                {"encryption", "Your messaging app scrambles messages so only the recipient can read them."},
                {"2fa", "A text message with a code required after entering your password."},
                {"updates", "Installing the latest Windows security patches."},
                {"public wi-fi", "Avoiding online banking while connected to free airport Wi-Fi."}
            };

            return examples.ContainsKey(topic) ? examples[topic] : "No example available.";
        }


        // Randomly selects a tip from the topic, avoiding repeats until all tips are used.
        private string GetRandomTip(string topic)
        {
            if (!topicTips.ContainsKey(topic))
                return "Sorry, I don't have tips on that topic yet.";

            if (!usedTopicTips.ContainsKey(topic))
                usedTopicTips[topic] = new HashSet<string>();

            var unusedTips = topicTips[topic].Where(t => !usedTopicTips[topic].Contains(t)).ToList();

            if (unusedTips.Count == 0)
            {
                usedTopicTips[topic].Clear();
                unusedTips = topicTips[topic].ToList();
            }

            var selectedTip = unusedTips[random.Next(unusedTips.Count)];
            usedTopicTips[topic].Add(selectedTip);

            return selectedTip;
        }

        private string ShowHelpMenu()
        {
            return "🆘 I can help you with these cybersecurity topics:\n" +
                   string.Join(", ", topicTips.Keys.OrderBy(k => k)) +
                   "\nAsk me about any of these or say 'tip' for a daily security tip.";
        }

        private string GetGoodbyeMessage()
        {
            var summary = "";
            if (conversationTopics.Count > 0)
            {
                summary = "Here's a quick recap of topics we've covered:\n";
                foreach (var topic in conversationTopics)
                    summary += $"- {topic}\n";
            }

            return $"👋 Goodbye, {userName}! {summary}Stay safe online! 🔐";
        }
    }
}
