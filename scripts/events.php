<?php

function get_events_data()
{
	$files_output = [];

	if (!isset($_POST['vote_event_id']))
	{
		$max_files = 10;
		if (isset($_POST['max_files']))
			$max_files = intval($_POST['max_files']);

		$files = scandir("wp-content/asambleapp/batekapp/database/events");

		foreach ($files as $file)
		{
			if (strlen($file) >= 11 && strlen($file) <= 14 && $file != 'events_lock')
			{
				$files_output[] = $file;

				if (count($files_output) >= $max_files)
					break;
			}
		}
	}
	else
		$files_output[] = "event_" . $_POST['vote_event_id'] . ".xml";

	foreach ($files_output as $file)
	{
		$eventDatabasePath = "wp-content/asambleapp/batekapp/database/events/" . $file;
		$xpath = utils_get_xpath($eventDatabasePath)[1];

		if ($xpath == false)
			return "Couldn't load event database '" . $file . "'.";

		$rootNode = $xpath->query("/event")[0];

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
			}
		}

		echo '_DBEND_';
	}

	echo '|';
	return 'NONE';
}

function set_event_vote($user_data)
{
	$lock_path = 'wp-content/asambleapp/batekapp/database/events/lock';

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
	$eventDatabasePath = "wp-content/asambleapp/batekapp/database/events/event_" . $_POST['vote_event_id'] . ".xml";

	$xdata = utils_get_xpath($eventDatabasePath);

	if ($xdata == false)
		return return_error($lock_path, "Couldn't load event database");

	$xmlDoc = $xdata[0];
	$xpath = $xdata[1];

	$user_vote_node = $xpath->query("/event/options/" . $_POST['vote_type']);

	if ($user_vote_node->length != 1)
		return return_error($lock_path, "Vote type '" . $_POST['vote_type'] . "' not recognized [" . $user_vote_node->length . "]");

	$user_vote_node = $user_vote_node[0];

	// Remove previous vote if existent
	$options_node = $xpath->query("/event/options")[0];

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
	$xmlDoc->save($eventDatabasePath);

	// Remove Lock
	set_lock($lock_path, '0');

	return get_events_data();
}

?>