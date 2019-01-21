# MovieNightBot

A simple discord bot for tracking movie suggestions and generating a random vote for unwatched movies.

# Prerequisites
* Discord.Net
* Newtonsoft.Json
* .Net Core
	
	
# Commands

All commands must be prefixed with m! OR you can @ the bot directly. Commands marked with ^ require the user to have the role "Movie Master".
* help - DMs the sender of the commands this list of commands.
* suggest [Title] - Adds the movie to the suggestions list. There is a chance this movie will now show up on future votes.
* watched - Lists all movies that have been watched.
* suggested - Lists all movies that have been suggested.
* setwatched [Title] - Sets the specified movie as having been watched. This movie will not show up on future votes.
* unwatch [Title] - Removes the specified movie from the watched list.
* remove [Title] - Removes the specified movie from the suggestions list. *^*
* beginvote - Selects a number of random movie suggestions to be voted on.
* showvote - Ends the currently running vote and displays the winning vote.
* vote [Title Number] - During a vote cycle users cast votes using this command.
* moviecount [Number] - Sets the number of movies that will show up on a vote. *^*

Currently, votes that end with a tie will automatically restart the vote with a new random set of movies.