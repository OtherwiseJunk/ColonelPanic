using DartsDiscordBots.Modules.Jackbox.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DartsDiscordBots.Modules.Jackbox
{
	public class JackboxConstants : IJackboxConstants
	{
		int JackboxMaxVersion { get; set; } = 7;

		public List<string> JackboxOneGames { get; set; } = new List<string>()
			{
				":smirk: You Don't Know Jack 2015 (1-4 Know-It-Alls) (No Audience)",
				":liar: Fibbage XL (2-8 $#%!ing Liars) (No Audience)",
				":pencil: Drawful (3-8 Doodlers) (No Audience)",
				":potato: Word Spud (2-8 Potatos) (No Audience)",
				":fly: Lie Swatter (1-100 Insects) (No Audience)",
			};
		public List<string> JackboxTwoGames { get; set; } = new List<string>()
			{
				":confused: Fibbage 2 (2-8 $#%!ing Liars)",
				":money_mouth: Bidiots (3-6 Big Spenders) (No Audience)",
				":bomb: Bomb Corp. (1-4 Defusers) (No Audience)",
				":ear: Earwax (3-8 Q-Tippers)",
				":joy: Quiplash XL (3-8 EXTRA LARGE funny people)"
			};
		public List<string> JackboxThreeGames { get; set; } = new List<string>()
			{
				":laughing: Quiplash 2 (3-8 Funny People)",
				":scream: Triva Murder Party (1-8 Soon-To-Be-Corpses)",
				":spy: Gusspionage (2-8 Undercover Guessers)",
				":shirt: Tee K.O. (3-8 Silk-screeners)",
				":japanese_goblin: Fakin' It (3-6 BIG PHONIES)"
			};
		public List<string> JackboxFourGames { get; set; } = new List<string>()
			{
				":face_with_raised_eyebrow: Fibbage 3 (2-8 $#%!ing LIars)",
				":desktop: Survive the Internet (3-8 NEETs)",
				":purple_heart: Monster Seeking Monsters (3-7 Hot Abominations)",
				":medal: Bracketeering (3-16 Mad Marchers)",
				":paintbrush: Civic Doodle (3-8 Drawers)"
			};
		public List<string> JackboxFiveGames { get; set; } = new List<string>()
			{
				":nerd: You Dont Know Jack: Full Stream (1-8 Know-It-Alls)",
				":cat: Split the Room (3-8 Trolls)",
				":blue_square: Patently Stupid (3-8 Idiots)",
				":microphone: Mad Verse City (3-8 Rappers)",
				":alien: Zeeple Dome (1-6 Abductees) (No Audience)"
			};
		public List<string> JackboxSixGames { get; set; } = new List<string>()
			{
				":dagger: Trivia Murder Party 2 (1-8 Soon-To-Be-Corpses)",
				":bar_chart: Role Models (3-6 Impressionable Youths)",
				":rowboat: Joke Boat (3-8 Comedians)",
				":blue_book: Dictionarium (3-8 Wordcrafters)",
				":black_square_button: Push The Button (4-10 Button-Pushers) (No Audience)"
			};

		public List<string> JackboxSevenGames { get; set; } = new List<string>()
			{
				":loud_sound: Talking Points (3-8 Pundits)",
				":tongue: Blather 'Round (2-6 Blathermouths)",
				":imp: The Devils and the Details (3-8 Devils)",
				":muscle: Champ'd Up (3-8 Champs)",
				":rofl: Quiplash 3 (3-8 Funny People)"
			};
		public Dictionary<int, List<string>> JackboxGameListByNumber { get; set; }
		int IJackboxConstants.JackboxMaxVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public JackboxConstants()
		{
			JackboxGameListByNumber = new Dictionary<int, List<string>>()
			{
				{ 1, JackboxOneGames},
				{ 2, JackboxTwoGames},
				{ 3, JackboxThreeGames},
				{ 4, JackboxFourGames},
				{ 5, JackboxFiveGames},
				{ 6, JackboxSixGames},
				{ 7, JackboxSevenGames},
			};

		}
	}
}
