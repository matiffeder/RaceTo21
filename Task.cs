using System;
namespace RaceTo21
{
    //value cannot be changed and added in enum
    //it's a simple way to save status
	public enum Task
	{
        GetGamesGoal,
        GetNumberOfPlayers,
		GetNames,
        IntroducePlayers,
        PlayerTurn,
        CheckForEnd,
        GameOver,
    }
}

