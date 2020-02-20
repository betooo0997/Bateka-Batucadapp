<?php

include "wp-content/asambleapp/utils.php";
include "wp-content/asambleapp/batekapp/users.php";
include "wp-content/asambleapp/batekapp/polls.php";
include "wp-content/asambleapp/batekapp/news.php";
include "wp-content/asambleapp/batekapp/events.php";
include "wp-content/asambleapp/batekapp/docs.php";

function handleRequest()
{	
    $databasePath = "wp-content/asambleapp/batekapp/database/user_database.xml";
	$xpath = utils_get_xpath($databasePath)[1];

	if ($xpath != false)
	{
		$user_data = get_user_nodes($xpath);

		if ($user_data != false && $user_data["psswd"]->nodeValue == $_POST['psswd'])
		{
			echo 'VERIFIED.|';

			switch($_POST['REQUEST_TYPE'])
			{
				case 'get_user_data':
					return get_user_data($user_data, $xpath);

				case 'get_polls':
					return get_poll_data();

				case 'set_poll_vote':
					return set_poll_vote($user_data);

				case 'set_poll':
					return set_poll();

				case 'get_news':
					return get_news_data();

				case 'get_events':
					return get_events_data();

				case 'set_event_vote':
					return set_event_vote($user_data);

				case 'get_docs':
					return get_docs_data();

				default:
					return 'REQUEST_TYPE "' . $_POST['REQUEST_TYPE'] . '" not understood.';
			}
		}
		else echo 'WRONG_CREDENTIALS.';
		return 'NONE';
	}
	return "Couldn't load user_database.xml";	
}

?>