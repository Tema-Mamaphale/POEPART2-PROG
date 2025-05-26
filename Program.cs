using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LunaCyberSecurityChatbot;

namespace Chatbot_Luna
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            ShowIntro();
            Audio.Player();

            LunaBot luna = new LunaBot();
            var moodMgr = new MoodManager();

            // Ask the user for their name
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(luna.GreetUser());
            Console.ResetColor();

            // Read user name input and save it in memory for later use
            string name = Console.ReadLine();
            Memory.UserName = name;
            luna.RememberUserName(name);

            // Welcome the user and ask about their interests
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(luna.WelcomeUser());
            Console.ResetColor();

            string interest = Console.ReadLine();
            Memory.UserInterest = interest;
            luna.RememberUserInterest(interest);

            // Main chat loop - runs until user exits
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"\n{Memory.UserName} 💬: ");
                    string input = Console.ReadLine();
                    Console.ResetColor();

                    if (input == null || string.IsNullOrWhiteSpace(input))
                    {
                        ShowEmptyInputWarning();
                        continue;
                    }

                    // Normalize input by trimming and converting to lowercase for easier processing
                    input = input.Trim().ToLower();

                    if (IsGibberish(input))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Luna 🌙: Hmm, that doesn't look like a real question. Try typing something clearer! 🤔");
                        Console.ResetColor();
                        continue;
                    }

                    if (IsExitCommand(input))
                    {
                        ShowDetailedExitMessage();
                        break;
                    }

                    // Detect mood only once per input
                    string mood = moodMgr.DetectMood(input);

                    string topic = DetectTopic(input);

                    if (!string.IsNullOrEmpty(topic) && !Memory.SeenTips.Contains(topic))
                    {
                        Memory.SeenTips.Add(topic);
                    }

                    string response;

                    // If user input looks like a follow-up and a topic is set, continue that conversation

                    if (IsFollowUp(input) && !string.IsNullOrEmpty(Memory.CurrentTopic))
                    {
                        response = luna.ProcessInput(input, mood, Memory.CurrentTopic);
                    }
                    else
                    {
                        response = luna.ProcessInput(input, mood, topic);

                        if (!string.IsNullOrEmpty(topic))
                            Memory.CurrentTopic = topic;
                    }

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Luna 🌙: {response}");
                    Console.ResetColor();

                    // Only print mood response if detected and different from neutral
                    if (!string.IsNullOrEmpty(mood))
                    {
                        string moodResponse = string.IsNullOrEmpty(topic)
                            ? moodMgr.GetMoodResponse(mood)
                            : moodMgr.GetMoodResponse(mood, topic);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Luna 🌙: {moodResponse}");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Luna 🌙: Oops! Something went wrong. ({ex.Message}) 😢");
                    Console.WriteLine("But don't worry — I'm still here. Try typing again!");
                    Console.ResetColor();
                }
            }
        }

        private static string DetectTopic(string input)
        {
            var topics = new List<string> {
                "phishing", "passwords", "malware", "vpn", "ransomware",
                "firewalls", "social engineering", "data breaches",
                "encryption", "2fa", "updates", "public wi-fi"
            };

            input = input.ToLower();
            foreach (var topic in topics)
            {
                if (input.Contains(topic))
                    return topic;
            }

            return null;
        }

        private static bool IsFollowUp(string input)
        {
            var followUps = new List<string>
            {
                "tell me more", "more", "what else", "another tip",
                "help me understand", "explain that", "what should i do",
                "any advice", "expand", "go on"
            };

            input = input.ToLower();
            foreach (var phrase in followUps)
            {
                if (input.Contains(phrase))
                    return true;
            }
            return false;
        }

        private static void ShowIntro()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
██╗     ██╗   ██╗███╗   ██╗ █████╗    
██║     ██║   ██║████╗  ██║██╔══██╗ 
██║     ██║   ██║██╔██╗ ██║███████║ 
██║     ██║   ██║██║╚██╗██║██╔══██║   
███████╗╚██████╔╝██║ ╚████║██║  ██║██╗ 
╚══════╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝
        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⣛⡫⠉⣉⡉⣭⣭⣽⣛⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⢟⣥⡶⢟⢋⣴⣿⣿⣿⣮⠛⢻⡿⠿⢶⣝⡻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⣫⣴⣿⢏⡴⢣⣿⣿⣿⠿⢿⣿⣧⡐⡾⢟⣒⡢⠙⢮⡻⣿⣿⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⢟⣼⣿⡿⣡⣿⢣⣿⣿⠏⣴⡿⠖⠊⠻⣿⣿⣿⣿⣿⣷⣆⢻⡜⣿⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⣿⢏⣾⣿⢏⣼⣿⢣⣿⣿⡏⢰⣶⣶⣶⣦⣤⡙⠋⠉⠙⢻⣿⣿⡎⣧⢹⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⡏⣼⣿⢃⣾⣿⢣⣿⣿⡟⢀⣛⣿⣿⣿⣿⣟⣉⣈⣙⣓⠀⣿⣿⡇⢿⢸⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⣿⢸⡿⣡⣿⡿⢁⣾⣿⣿⢣⣿⣟⢿⣿⣿⣿⢩⣿⣿⣿⣿⡇⣿⣿⡇⣸⢸⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⡇⠚⣴⣿⡟⢀⣾⣿⣿⠇⡚⢿⣿⡊⣿⣿⣿⢠⣿⠟⣛⠟⡃⢸⣿⡇⠁⢸⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⢇⣾⣿⠏⣴⣿⣿⣿⡟⠀⠙⠿⣿⣧⣸⣿⣿⣿⡷⠋⠁⠀⠀⢸⣿⡇⠄⣾⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⢏⣾⣿⢋⣾⣿⣿⣿⡿⢁⠀⠀⠀⢙⣿⣿⣿⣿⡟⠀⠀⠀⣰⠷⢸⣿⡅⡀⢹⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⡏⣼⡿⢣⣿⣿⣿⣿⠟⣡⣶⣶⣴⣶⣿⣿⣿⣿⣿⣿⣷⣶⣶⣶⣾⢸⣿⠀⠉⢸⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣇⢿⢣⣿⣿⣿⠟⢡⠘⣿⣿⣿⣿⣿⣿⡿⢻⡿⠻⣿⣿⣿⣿⣿⣟⠈⣿⡆⢺⡜⣿⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⡌⢸⣿⣿⢫⣾⣷⣦⢸⣿⣿⣿⣿⣿⣷⣤⣤⣴⣿⣿⣿⣿⣿⡟⠀⢻⣷⠀⢿⡹⣿⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⣿⡎⢿⣷⠸⣿⣿⣿⡆⠹⣿⣿⣯⣙⠋⣁⣉⠉⢛⣽⣿⣿⠟⣡⡄⠸⣿⣧⠈⣧⢻⣿⣿⣿⣿
        ⣿⣿⣿⣿⣿⡿⢰⢶⣭⣓⣈⡛⢸⣧⢸⣮⡙⠿⣿⣷⣤⣠⣶⣿⡿⢛⣡⣾⣿⡇⣷⣝⢿⣷⣸⣎⢿⣿⣿⣿
        ⣿⣿⣿⣿⡟⢁⣿⢸⣿⣿⣿⡏⣾⡇⠸⣿⣿⡆⢠⣝⣛⣟⣋⣥⢠⣿⣿⣿⣿⢹⣿⣿⣷⣾⡇⣿⣯⠻⣿⣿
        ⣿⣿⣿⠟⢀⡾⣡⢸⣿⣿⠟⢰⣿⠃⢰⣿⣿⣿⠈⠉⠛⠛⠋⠁⢸⣿⣿⣿⡿⣸⣿⣿⣿⣿⡇⣻⣿⣷⡹⣿
        ⣿⣿⠏⢠⡞⣱⡿⠘⠛⠋⠆⢘⣛⠠⠿⠿⠿⠋⠀⠀⠀⠀⠀⠀⠈⠻⠿⠿⠇⢛⣛⠻⠿⠛⠓⢿⣿⡟⣷⡸
        ⣿⡏⢠⢏⣼⡿⠁⠀⠀⠀⠀⣾⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⠀⠀⠀⠀⠀⢻⣿⡹⣷⢻
        ⡿⣰⣇⣾⣿⠁⠀⠀⠀⠀⠀⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⢿⣇⢿⠸
        ⡇⣿⢸⣿⡏⠀⠀⠀⠀⠀⢰⣿⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠸⣟⢸⠀
        ⡇⣿⣾⣿⠁⠀⠀⠀⠀⠀⣼⣿⣿⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⡀⠀⠀⠀⠀⠀⢿⠈⢰
🌙 Hello! I'm Luna, your friendly cybersecurity awareness guide! 💻🛡️
💡 Type 'help' at any time to see what I can do!
🚪 Type 'exit', 'quit', or 'bye' to leave the chat.
");
            Console.ResetColor();
        }

        private static void ShowEmptyInputWarning()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Luna 🌙: Please type something 😊");
            Console.ResetColor();
        }

        private static bool IsExitCommand(string input)
        {
            return input == "exit" || input == "quit" || input == "bye";
        }

        private static bool IsGibberish(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            if (input.Length <= 2 && !char.IsLetterOrDigit(input[0]))
                return true;

            string trimmed = input.Trim(new char[] { '!', '@', '#', '$', '%', '^', '&', '*', '-', '_', '=', '+', '.', ',', '?', ';', ':' });
            return string.IsNullOrWhiteSpace(trimmed);
        }

        private static void ShowDetailedExitMessage()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nLuna 🌙: Thanks for chatting, {Memory.UserName}! 👋");

            if (Memory.SeenTips.Count > 0)
            {
                Console.WriteLine("Today you explored:");
                foreach (var topic in Memory.SeenTips)
                    Console.WriteLine($"   🔹 {topic}");
            }

            if (!string.IsNullOrEmpty(Memory.UserInterest))
            {
                Console.WriteLine($"\nSince you're interested in {Memory.UserInterest},");
                Console.WriteLine("I recommend reviewing your account settings and privacy preferences regularly. 🔐");
            }

            Console.WriteLine("\nStay sharp online — and remember: Think before you click! 🛡️");
            Console.ResetColor();
        }
    }
}
