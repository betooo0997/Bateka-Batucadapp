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
		echo 'VERIFIED.|';
		$_POST['REQUEST_TYPE'] = 'set_poll';

		switch($_POST['REQUEST_TYPE'])
		{
			case 'get_user_data':
				return get_user_data($con);

			case 'get_polls':
				return get_poll_data($con);

			case 'set_poll_vote':
				return set_poll_vote($con);

			case 'set_poll':
				return set_poll($con);

			case 'get_events':
				return get_event_data($con);

			case 'set_event_vote':
				return set_event_vote($con);

			case 'set_event':
				return set_event($con);

			case 'get_news':
				return get_news_data($con);

			case 'get_docs':
				return get_docs_data($con);

			case 'get_version':
				echo "1.1";
				return 'NONE';

			case 'get_rhythms':
				return get_rhythms_data($con);

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
	$user_name = 'dbu14967';
	$password = 'DrTgcePl06K#';
	$connect = mysqli_connect($host_name, $user_name, $password, $database);

	if (mysqli_connect_errno()) 
		return false;
		//return 'Error al conectar con servidor MySQL: ' . mysqli_connect_error();
	else 
		return $connect;
}

function verified_user($con)
{
	/*$_POST['username'] = 'beto';
	$_POST['psswd'] = '1234567891011121';*/

	$query = "SELECT salt, hash, id FROM users WHERE username='" . $_POST['username'] . "';";
	$result = mysqli_query($con, $query);
	$object = mysqli_fetch_object($result);

	$salt = $object->salt;
	$hash = $object->hash;
	$_POST['id'] = $object->id;

	return md5($_POST['psswd'] . $salt) == $hash;
}

?>