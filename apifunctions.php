<?php

include "wp-content/asambleapp/utils.php";

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
				case 'get_data':
					return get_user_data($user_data, $xpath);

				case 'get_polls':
					return get_poll_data();

				case 'set_poll_vote':
					return set_poll_vote($user_data);

				case 'set_poll':
					return set_poll();

				default:
					echo 'REQUEST_TYPE' . $_POST['REQUEST_TYPE'] . 'not understood';
					break;
			}
		}
		else echo 'WRONG_CREDENTIALS.';
		return 'NONE';
	}
	return "Couldn't load user_database.xml";	
}

function get_user_nodes($xpath)
{
	$userNode = $xpath->query("/users/user[username='" . $_POST['username'] . "']");

	if ($userNode->length > 0)
		$userNode = $userNode->item(0);
	else return false;

	$user_nodes = [];

	foreach($userNode->childNodes as $child)
        $user_nodes[$child->nodeName] = $child;

	return $user_nodes;
}

function get_user_data($user_data, $xpath)
{
	// Echo own user data
	foreach ($user_data as $key => $value)
		echo $key . '$' . $user_data[$key]->nodeValue . '#';

	echo '%';

	// Echo other user data
	$userNodes = $xpath->query("/users/user");

	foreach ($userNodes as $userNode)
	{
		foreach($userNode->childNodes as $child)
			echo $child->nodeName . '$' . $child->nodeValue . '#';

		echo '%';
	}

	return 'NONE';
}

function get_poll_data()
{
	$files_output = [];

	if (!isset($_POST['vote_poll_id']))
	{
		$files = scandir("wp-content/asambleapp/batekapp/database/polls");

		foreach ($files as $file)
		{
			if (strlen($file) >= 10 && strlen($file) <= 14)
			{
				$files_output[] = $file;

				if (count($files_output) >= 5)
					break;
			}
		}
	}
	else
		$files_output[] = "poll_" . $_POST['vote_poll_id'] . ".xml";

	foreach ($files_output as $file)
	{
		$pollDatabasePath = "wp-content/asambleapp/batekapp/database/polls/" . $file;
		$xpath = utils_get_xpath($pollDatabasePath)[1];

		if ($xpath == false)
			return "Couldn't load user_database.xml";

		$rootNode = $xpath->query("/poll")[0];
		$poll_nodes = [];

		foreach ($rootNode->childNodes as $child)
		{
			switch($child->nodeName)
			{
				default:
					echo $child->nodeName . '$' . $child->nodeValue . '#';
					break;

				case 'options':
					echo 'options$';
					$options = $child->childNodes;

					foreach ($options as $option)
						echo $option->nodeName . '@,' . $option->nodeValue . '+';

					echo '#';
					break;

				case 'comments':
					echo '\COMMENTS';
					$comments = $child->childNodes;

					foreach ($comments as $comment)
					{
						foreach ($comment->childNodes as $data)
							echo $data->nodeName . '^' . $data->nodeValue . '~';
						echo '#';
					}
					break;
			}
		}

		echo '_PDBEND_';
	}

	echo '|';
	return 'NONE';
}

// Removes an user_id from a vote list. (E.g. "1,2,3,4" -> user_id = 2 -> "1,3,4")
function remove_vote($votes, $vote_to_remove)
{
	$votes = str_replace($vote_to_remove, "", $votes);
	$votes = str_replace(",,", ",", $votes);

	if($votes[0] == ',')
		$votes = substr($votes, 1);

	if($votes[strlen($votes) - 1] == ',')
		$votes = substr($votes, 0, strlen($votes) - 1);

	return $votes;
}

function set_poll_vote($user_data)
{
	// Check if database is locked
	if (!file_exists('wp-content/asambleapp/batekapp/database/lock'))
		echo 'FILENOTEXIST#';

	$file_r = fopen('wp-content/asambleapp/batekapp/database/lock', 'r');
	$content = fread($file_r, 1);
	fclose($file_r);

	$attempts = 0;
	while ($content == '1')
	{
		if ($attempts > 0) 
			return 'Timeout database lock';

		sleep(1);

		$file_r = fopen('wp-content/asambleapp/batekapp/database/lock', 'r');
		$content = fread($file_r, 1);
		fclose($file_r);
		$attempts += 1;
	}

	// Lock database
	set_lock('1');

	// Load database
	$user_id = $user_data['id']->nodeValue;
	$pollDatabasePath = "wp-content/asambleapp/batekapp/database/polls/poll_" . $_POST['vote_poll_id'] . ".xml";

	$xdata = utils_get_xpath($pollDatabasePath);

	if ($xdata == false)
		return return_error("Couldn't load poll database");

	$xmlDoc = $xdata[0];
	$xpath = $xdata[1];

	$user_vote_node = $xpath->query("/poll/options/" . $_POST['vote_type']);

	if ($user_vote_node->length != 1)
		return return_error("Vote type '" . $_POST['vote_type'] . "' not recognized [" . $user_vote_node->length . "]");

	$user_vote_node = $user_vote_node[0];

	// Remove previous vote if existent
	$options_node = $xpath->query("/poll/options")[0];

	foreach ($options_node->childNodes as $option_node)
	{
		$option_node_content = remove_vote($option_node->nodeValue, $user_id);
		$option_node->nodeValue = $option_node_content;
	}

	// Add new vote
	$user_vote_node_content = $user_vote_node->nodeValue;

	if (strlen($user_vote_node_content) > 0) $user_vote_node_content .= ',';
	$user_vote_node_content .= $user_id;

	$user_vote_node->nodeValue = $user_vote_node_content;
	$xmlDoc->save($pollDatabasePath);

	// Remove Lock
	set_lock('0');

	return get_poll_data();
}

function set_poll()
{
	echo $_POST['poll_id'] . $_POST['poll_data'];
	return 'NONE';
}

?>