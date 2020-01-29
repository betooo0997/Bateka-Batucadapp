<?php

function get_poll_data()
{
	$files_output = [];

	if (!isset($_POST['vote_poll_id']))
	{
		$max_files = 5;
		if (isset($_POST['max_files']))
			$max_files = intval($_POST['max_files']);

		$files = scandir("wp-content/asambleapp/batekapp/database/polls");

		foreach ($files as $file)
		{
			if (strlen($file) >= 10 && strlen($file) <= 14 && $file != 'polls_lock')
			{
				$files_output[] = $file;

				if (count($files_output) >= $max_files)
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
			return "Couldn't load poll database '" . $file . "'.";

		$rootNode = $xpath->query("/poll")[0];

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

		echo '_DBEND_';
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
	$lock_path = 'wp-content/asambleapp/batekapp/database/polls/polls_lock';

	// Check if database is locked
	if (!file_exists($lock_path))
		echo 'FILENOTEXIST#';

	$file_r = fopen($lock_path, 'r');
	$content = fread($file_r, 1);
	fclose($file_r);

	$attempts = 0;
	while ($content == '1')
	{
		if ($attempts > 0) 
			return 'Timeout database lock';

		sleep(1);

		$file_r = fopen($lock_path, 'r');
		$content = fread($file_r, 1);
		fclose($file_r);
		$attempts += 1;
	}

	// Lock database
	set_lock($lock_path, '1');

	// Load database
	$user_id = $user_data['id']->nodeValue;
	$pollDatabasePath = "wp-content/asambleapp/batekapp/database/polls/poll_" . $_POST['vote_poll_id'] . ".xml";

	$xdata = utils_get_xpath($pollDatabasePath);

	if ($xdata == false)
		return return_error($lock_path, "Couldn't load poll database");

	$xmlDoc = $xdata[0];
	$xpath = $xdata[1];

	$user_vote_node = $xpath->query("/poll/options/" . $_POST['vote_type']);

	if ($user_vote_node->length != 1)
		return return_error($lock_path, "Vote type '" . $_POST['vote_type'] . "' not recognized [" . $user_vote_node->length . "]");

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
	set_lock($lock_path, '0');

	return get_poll_data();
}

function set_poll()
{
	echo $_POST['poll_id'] . $_POST['poll_data'];
	return 'NONE';
}

?>