using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot_Luna
{
    public class Audio
    {
        // Play greeting WAV file (if it exists)
        public static void Player()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("C:\\Users\\lab_services_student\\Downloads\\Luna_Chatbot.wav");
                player.PlaySync();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("🔇 Luna 🌙: (Audio greeting not found. Skipping...)");
                Console.ResetColor();
            }
        }
    }
}