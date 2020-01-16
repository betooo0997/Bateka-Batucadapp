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
					break;

				case 'get_polls':
					return get_poll_data();
					break;

				case 'set_poll_vote':
					return set_poll_vote($user_data);
					break;

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
	$files = scandir("wp-content/asambleapp/batekapp/database/polls");
	$files_output = [];

	foreach ($files as $file)
	{
		if (strlen($file) >= 10 && strlen($file) <= 14)
		{
			$files_output[] = $file;

			if (count($files_output) >= 5)
				break;
		}
	}

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
			if ($child->nodeName != 'comments')
				echo $child->nodeName . '$' . $child->nodeValue . '#';
			else
			{
				echo '\COMMENTS';
				$comments = $child->childNodes;

				foreach ($comments as $comment)
				{
					foreach ($comment->childNodes as $data)
						echo $data->nodeName . '^' . $data->nodeValue . '~';
					echo '#';
				}
			}
		}

		echo '_PDBEND_';
	}

	echo '|';
	return 'NONE';
}

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
	$user_id = $user_data['id']->nodeValue;
	$pollDatabasePath = "wp-content/asambleapp/batekapp/database/polls/poll_" . $_POST['vote_pollid'] . ".xml";

	$xdata = utils_get_xpath($pollDatabasePath);

	if ($xdata == false)
		return "Couldn't load poll database";

	$xmlDoc = $xdata[0];
	$xpath = $xdata[1];

	if ($_POST['vote_type'] != 'favour' && $_POST['vote_type'] != 'abstention' 
	&& $_POST['vote_type'] != 'opposed' && $_POST['vote_type'] != 'blank')
		return "Vote type not recognized";

	// Removing previous vote if existent
	$favour_node		= $xpath->query("/poll/favour")[0];
	$abstention_node	= $xpath->query("/poll/abstention")[0];
	$opposed_node		= $xpath->query("/poll/opposed")[0];
	$blank_node			= $xpath->query("/poll/blank")[0];

	$favour_content		= remove_vote($favour_node->nodeValue, $user_id);
	$abstention_content	= remove_vote($abstention_node->nodeValue, $user_id);
	$opposed_content	= remove_vote($opposed_node->nodeValue, $user_id);
	$blank_content		= remove_vote($blank_node->nodeValue, $user_id);

	$favour_node->nodeValue		= $favour_content;
	$abstention_node->nodeValue = $abstention_content;
	$opposed_node->nodeValue	= $opposed_content;
	$blank_node->nodeValue		= $blank_content;

	// Add new vote
	$user_vote_node			= $xpath->query("/poll/" . $_POST['vote_type'])[0];
	$user_vote_node_content = $user_vote_node->nodeValue;

	if (strlen($user_vote_node_content) > 0) $user_vote_node_content .= ',';
	$user_vote_node_content .= $user_id;

	$user_vote_node->nodeValue = $user_vote_node_content;
	$xmlDoc->save($pollDatabasePath);
	return 'NONE';
}

?>