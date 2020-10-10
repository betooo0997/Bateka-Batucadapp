<?php

include "wp-content/asambleapp/utils.php";
include "wp-content/asambleapp/batekapp/users.php";
include "wp-content/asambleapp/batekapp/polls.php";
include "wp-content/asambleapp/batekapp/news.php";
include "wp-content/asambleapp/batekapp/events.php";
include "wp-content/asambleapp/batekapp/docs.php";
include "wp-content/asambleapp/batekapp/rhythms.php";

function handleRequest()
{	
    $con = SQL_connect();

	if ($con == false)
		return 'Failed to connect to database';

	if (verified_user($con))
	{
		echo 'VERIFIED.~';
		// $_POST['REQUEST_TYPE'] = 'set_rhythms';

		switch($_POST['REQUEST_TYPE'])
		{
			case 'get_user_data':
				return get_user_data($con);

			// Polls
			case 'get_polls':
				return get_data($con, "polls");

			case 'set_poll_vote':
				return set_poll_vote($con);

			case 'set_poll':
				return set_poll($con);

			case 'delete_poll':
				return delete_data($con, "polls");

			// Events
			case 'get_events':
				return get_data($con, "events");

			case 'set_event_vote':
				return set_event_vote($con);

			case 'set_event':
				return set_event($con);

			case 'delete_event':
				return delete_data($con, "events");

			// News
			case 'get_news':
				return get_data($con, "news");

			case 'set_news':
				return set_news_entry($con);

			case 'set_news_seen':
				return set_news_entry_seen($con);

			case 'delete_news':
				return delete_data($con, "news");

			// Docs
			case 'get_docs':
				return get_data($con, "docs");

			case 'set_doc':
				return set_doc($con);

			case 'delete_doc':
				return delete_data($con, "docs", $_POST['id']);

			// Rhythms
			case 'get_rhythms':
				return get_data($con, "rhythms");

			case 'set_rhythm':
				return set_rhythm($con, "rhythms");

			case 'delete_rhythm':
				return delete_data($con, "rhythms", $_POST['id']);

			// Other
			case 'get_version':
				echo "0.41";
				return 'NONE';

			default:
				return 'REQUEST_TYPE "' . $_POST['REQUEST_TYPE'] . '" not understood.';
		}
	}
	else 
		echo 'WRONG_CREDENTIALS.';

	return 'NONE';
}

function SQL_connect()
{
	$host_name = 'db5000391040.hosting-data.io';
	$database = 'dbs375954';
	// $user_name = 'dbu14967';
	// $password = 'DrTgcePl06K#';
	$connect = mysqli_connect($host_name, $_POST['db_username'], $_POST['db_password'], $database);

	if (mysqli_connect_errno()) 
		return false;
		//return 'Error al conectar con servidor MySQL: ' . mysqli_connect_error();
	else 
		return $connect;
}

function verified_user($con)
{
	// $_POST['username'] = 'beto';
	// $_POST['psswd'] = '1234567891011121';

	$query = "SELECT salt, hash, id FROM users WHERE username='" . $_POST['username'] . "';";
	$result = mysqli_query($con, $query);
	$object = mysqli_fetch_object($result);

	$salt = $object->salt;
	$hash = $object->hash;
	$_POST['id'] = $object->id;

	return md5($_POST['psswd'] . $salt) == $hash;
}

?>