using Godot;
using System;

public class BattleDialogueManager : Control
{
	//Returns a string used for a character's ability dialogue
	public string GetDialogue(string characterName, string targetName, float amount, string abilityName, bool likedPerformance, bool dislikedPerformace) 
	{
		switch (characterName)
		{
			case "Griff":
				{
					return Griff(targetName, amount, abilityName);
				}
			case "Slimette":
				{
					GD.Print("Using " + abilityName);
					return SlimeGuy(targetName, amount, abilityName);
				}
			//case "Slimette":
			//	{
			//		return Slimette(targetName, amount, abilityName);
			//	}
			case "Peeg":
				{
					return Peeg(targetName, amount, abilityName);
				}
			case "TumbleTrout":
				{
					return TumbleTrout(targetName, amount, abilityName);
				}
			case "FinnAudience":
				{
					return FinnAudience();
				}
			case "Finn":
				{
					return Finn(characterName, targetName, abilityName,likedPerformance, dislikedPerformace);
				}
			case "Rio":
				{
					return Rio(characterName, targetName, abilityName, likedPerformance, dislikedPerformace);
				}
		}
		return null;
	}

	string Griff(string targetName, float amount, string abilityName)
	{
		switch (abilityName)
		{
			case "Critique":
				{
					return "Griff expresses their thoughts on " + targetName + "'s performance. They lose " + amount + " energy";
				}
			case "Observe":
				{
					return "Not a word comes out of Griff, they simply watch...";
				}
			case "Rude Remark":
				{
					return "Griff calls " + targetName + " a dimwit! Their style was weakened!";
				}
			case "Domineering Gaze":
				{
					return "A wicked look is held by Griff's eyes." + targetName + "'s COnfidence was lowered!";
				}
			case "Enforce Preference":
				{
					return "Griff demands an artistic display, not absent-minded tricks!";
				}
			case "Hefty Expectations":
				{
					return "Only true art, performed with passion, will ever please Griff! They're less sensitive now!";
				}
		}
		return null;
	}

	string SlimeGuy(string targetName, float amount, string abilityName)
	{
		switch (abilityName)
		{
			case "Critique":
				{
					return "Slimette mumbles something about " + targetName + "'s perfomance. They lose " + amount + " energy!";
				}
			case "Observe":
				{
					return "Slimette bubbles away nervously watching your every move...";
				}
			case "A Slime's POV":
				{
					return "Slimette considers how " + targetName + "'s performance thus far relates to the world of slimes... They lose " + amount + " energy!";
				}
			case "Slimastic Defense":
				{
					return "Slimette simply does not pay attention to you. Not one bit...";
				}
			case "Slime Time":
				{
					return "A torrent of buttery goo exudes from Slimette... They're beginning to move faster!";
				}
			case "Slimence":
				{
					return "Slimette jiggles steadily... They're less entertained!";
				}
		}
		return null;
	}

	string Slimette(string targetName, float amount, string abilityName)
	{
		switch (abilityName)
		{
			case "Critique":
				{
					return "Slimette talks about " + targetName + "'s show thus far... They lose " + amount + " energy!";
				}
			case "Observe":
				{
					return "Slimette jiggles away steadily, watching you intensely...";
				}
			case "Slimure":
				{
					return "Woblling side-to-side, Slimette is quite distracting... " + targetName + "'s entertainment meter is reduced!";
				}
			case "Slimica":
				{
					return "Slimette jiggles rapidly, spreading goo everywhere! The entire audience reduced their entertainment by " + amount + ".";
				}
			case "Slimify":
				{
					return "Slimette gives a blue bubble to " + targetName + "... They became more critical!";
				}
			case "Slimen":
				{
					return "Slimette gives a red bubble to " + targetName + "... They became more insensitive!";
				}
		}
		return null;
	}

	string Peeg(string targetName, float amount, string abilityName)
	{
		switch (abilityName)
		{
			case "Critique":
				{
					return "With their broadened horizon, Peeg analyzes " + targetName + "'s performance... They lose " + amount + " energy!";
				}
			case "Observe":
				{
					return "With a distant stare, Peeg tunes you out in favour of nature and her beauty...";
				}
			case "PeegeeProtection":
				{
					return "Peeg bends, but does not break. The world's weight rolls right over them...";
				}
			case "PeegeeReaction":
				{
					return "Peeg simply floats there, yet their spirit is overwhelming!";
				}
			case "Fyshe'sWisdom":
				{
					return "Peeg preached about what the Great Fyshe in the sky thinks about your show... Peeg is now sheltering" + targetName + "!";
				}
			case "ExtraLard":
				{
					return "Peeg applies some rare lard to their supple body! They smell awful, but their Insensitivity increased!";
				}
		}
		return null;
	}

	string TumbleTrout(string targetName, float amount, string abilityName)
	{
		switch (abilityName)
		{
			case "Critique":
				{
					return "Tumble gives a thought or two on " + targetName + "'s performance... They lose " + amount + " energy!";
				}
			case "Observe":
				{
					return "Tumble Trout simply watches on from the confine of their barrel.";
				}
			case "PessimistPerspective":
				{
					return "Tumble Trout... doesn't like your show and doesn't think anyone ever should. He doesn't look too confident about it though.";
				}
			case "Shrunk":
				{
					return "Angrily, Tumble Trout reminds you that performance isn't anything special to him! " + targetName + " loses " + amount + " energy!";
				}
			case "Unbarrelable":
				{
					return "Tumble Trout has had enough! He no longer has any positive affinities!";
				}
			case "TumbleOn":
				{
					return "Tumble focuses on... tumbling. They're not doing a very good job of ignoring you.";
				}
		}
		return null;
	}

	string FinnAudience()
	{
		RandomNumberGenerator randomNumber = new RandomNumberGenerator();
		randomNumber.Randomize();
		int chosenNumber = randomNumber.RandiRange(1, 6);
		switch (chosenNumber)
		{
			case 1:
				{
					return "*sniffle* I- I'm sorry, I have to go, Rio...";
				}
			case 2:
				{
					return "Please don't pity me...";
				}
			case 3:
				{
					return "It's o-okay, I promise...";
				}
			case 4:
				{
					return "Rio... I'd explain i-if I could...";
				}
			case 5:
				{
					return "*whimper* It's just- I- I'm nervous...";
				}
			case 6:
				{
					return "Oh, Rio... you're so sweet...";
				}
		}
		return null;
	}

	string Finn(string characterName, string targetName, string abilityName, bool likedPerformace, bool dislikedPerformance)
	{
		switch (abilityName)
		{
			case "Performance":
				{
					return "Finn prepares a stirring song for " + targetName + "... " + PerformanceAffinity(characterName, abilityName, likedPerformace, dislikedPerformance);
				}
			case "Rest":
				{
					return "Finn focuses on themselves for a moment, breathing steadily...";
				}
			case "Stunning Song":
				{
					return "Finn delivers a challenge to " + targetName + " in the form of a song! They're too stunned to respond!";
				}
			case "Rehearse":
				{
					return "Finn prepares to sing, readying their body, mind, and songs! Finn's Spice has increased!";
				}
			case "Announce":
				{
					return "With a melodic voice, Finn sings the praise of " + targetName + " and their amazing talents! " + targetName + "'s Style increase!";
				}
			case "Inspire":
				{
					return "With a sweet song, Finn insists that " + targetName + " open their heart and mind for a but a moment... Their negative affinities have been converted!";
				}
			case "Amplify":
				{
					return "Insisting on " + targetName + "'s skills, Finn passes in their turn to sing alongside them! " + targetName + "'s Style, Spice, and Confidence temporarily increased!";
				}
		}
		return null;
	}

	string Rio(string characterName, string targetName, string abilityName, bool likedPerformace, bool dislikedPerformance)
	{
		switch (abilityName)
		{
			case "Performance":
				{
					return "With quick bumps and slick spins, Rio juggles his ball around and all over for " + targetName + "... " + PerformanceAffinity(characterName, abilityName, likedPerformace, dislikedPerformance);
				}
			case "Rest":
				{
					return "Kicking his ball up into his hands, Rio takes a minute to breathe...";
				}
			case "Triple Bounce":
				{
					return "Three bounces! Left knee, right knee, and one right on his head! A simple but powerful performance for " + targetName + "..." + PerformanceAffinity(characterName, abilityName, likedPerformace, dislikedPerformance);
				}
			case "Tumble Home":
				{
					return "Rio throws his ball up, catches it on his chest, and lets it slide down to his foot like water across a stone! Certainly a sleek performance for " + targetName + "... " + PerformanceAffinity(characterName, abilityName, likedPerformace, dislikedPerformance);
				}
			case "Rollout":
				{
					return "Bringing their ball up in one hand, Rio lets it roll on out from one arm down to the other!";
				}
			case "The Spin":
				{
					return "With a sly smile, Rio spins his ball on a single finger! His confidence rose sharply!";
				}
			case "Head Bobbing":
				{
					return "With a quick 'Hey!' Rio invites his friends to lose themselves in the rhythmic bouncing of the ball atop his head! The team regains some energy!";
				}
		}
		return null;
	}

	public string TutorialText(int roundNumber)
	{
		if (roundNumber == 1)
		{
			return "After everyone has used their actions, Audiences will 'Critique' the performance, draining a performer's 'Energy'. 'Critiques' are weaker if the Audience enjoys the 'Style' of your performer.";
		}
		else if(roundNumber == 2)
		{
			return "Talents can help you entertain an Audience, but cost 'Creativity' to use. Try 'Stunning Song' to help entertain an Audience.";
		}
		else if (roundNumber == 3)
		{
			return "Items can be used in Performances too. They can restore 'Creativity' and 'Energy' in a pinch.";
		}
		return null;
	}

	public string SlimetteTutorialText(int roundNumber)
	{
		if (roundNumber == 1)
		{
			return "J-just so you know, shows end once everyone is entertained. Please try to entertain me!";
		}
		else if(roundNumber == 2)
		{
			return "Be careful not to run out of 'Energy'! Shows that end early because of tired performers are no-good!";
		}
		else if(roundNumber == 3)
		{
			return "C-creativity is a big part of shows! You n-need 'Creativity' to show off your 'Talents', but you can run out of it too, so choose the right moments to use 'Talents'!";
		}
		return null;
	}

	string PerformanceAffinity(string characterName, string abilityName, bool likedPerformance, bool dislikedPerformance)
	{
		if (characterName == "Finn")
		{
			if (likedPerformance)
			{
				return "They're really enjoying it!";
			}
			else if (dislikedPerformance)
			{
				return "They're not too interested!";
			}
			else
			{
				return "They're beginning to like it!";
			}
		}
		else
		{
			if(abilityName == "Performance")
			{
				if (likedPerformance)
				{
					return "They're feeling really hyped!";
				}
				else if (dislikedPerformance)
				{
					return "They're not paying much attention!";
				}
				else
				{
					return "They're beginning to enjoy the show!";
				}
			}
			else if(abilityName == "Triple Bounce")
			{
				if (likedPerformance)
				{
					return "They're beginning to feel excited!";
				}
				else if (dislikedPerformance)
				{
					return "but they're not into it!";
				}
				else
				{
					return "Rio grabbed their attention!";
				}
			}
			else
			{
				if (likedPerformance)
				{
					return "They love it!";
				}
				else if (dislikedPerformance)
				{
					return "But they're not digging it!";
				}
				else
				{
					return "and they're really liking it!";
				}
			}
		}
	}
}
