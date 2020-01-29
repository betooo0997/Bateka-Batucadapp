<?php

function get_events_data()
{
	$files_output = [];

	$max_files = 5;
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
			}
		}

		echo '_DBEND_';
	}

	echo '|';
	return 'NONE';
}

?>