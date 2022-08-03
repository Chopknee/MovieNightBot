# MovieNightBot

A simple discord bot for tracking movie suggestions and generating a random vote for unwatched movies. The voting setup uses ranked ballots, meaning that each voter can choose multiple options. Each successive item the user picks carries less weight. The exact reduction of vote weight is determined by the number of items users are allowed to pick.

# Prerequisites
	
	
# Commands

* m!help 							- DMs the sender of the command this exact message. Now that's meta!
* m!suggest [Title] 				- Adds the supplied movie to the suggestions list. There is a chance this movie will now show up on future votes.
* m!watched [Page Number]			- Creates an embed that allows shows all watched movies. Use the reactions to navigate between pages. The argument is for what page to start on.
* m!suggested [Page Number]		- Creates an embed that allows shows all watched suggested. Use the reactions to navigate between pages. The argument is for what page to start on.
* m!set_watched [Title] 			- Sets the specified movie as having been watched. This movie will not show up on future votes.
* m!unwatch [Title] 				- Removes the specified movie from the watched list.
* m!remove [Title] 				- Removes the specified movie from the suggestions list.
* m!start_vote 					- Selects a number of random movie suggestions to be voted on.
* m!end_vote 						- Ends the currently running vote and displays the winning vote. Reacting to the vote embed with the stop sign, or octagonal sign will also end the vote.
* m!user_vote_count [Number] 		- Sets the number of movies users will be allowed to vote on. This cannot be greater than the number of movie options that shows on a vote.
* m!movie_option_count [Number] 	- Sets the number of movies that will show up on a vote.
* m!tie_option [option] 			- Sets how the bot handles tied votes.\n Option **breaker** will make a new vote using only the tied movies. Option random will make a new vote with a random selection of movies. WIP
* m!set_admin_role [Name] 		- Sets the name of the role that is allowed to run admin only commands in movie night bot. WARNING, you will not be able to run commands if you have no users with this name!!!
* m!get_admin_role [Name] 		- Gets the name of the role that is allowed to run admin only commands for Movie Night Bot. Use for emergencies.
* m!set_movie_time [Number] 		- Sets the hour when the movie will be watched. The hour is in UTC time zone, so convert accordingly. Valid range is 0 - 23 This time shows at the bottom of the vote embed.
