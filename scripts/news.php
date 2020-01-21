<?php

function get_news_data()
{
	$files_output = [];
	$files = scandir("wp-content/asambleapp/batekapp/database/news");

	$max_files = 5;
	if (isset($_POST['max_files']))
		$max_files = intval($_POST['max_files']);

	foreach ($files as $file)
	{
		if (strlen($file) >= 10 && strlen($file) <= 14)
		{
			$files_output[] = $file;

			if (count($files_output) >= $max_files)
				break;
		}
	}

	foreach ($files_output as $file)
	{
		$database_path = "wp-content/asambleapp/batekapp/database/news/" . $file;
		$xpath = utils_get_xpath($database_path)[1];

		if ($xpath == false)
			return "Couldn't load news database '" . $file . "'.";

		$rootNode = $xpath->query("/news")[0];

		foreach ($rootNode->childNodes as $child)
		{
			if($child->nodeName == 'imgs')
			{
				echo '\IMGS';
				foreach ($child->childNodes as $grandchild)
					echo $grandchild->nodeValue . '~';
				echo '#';
			}
			else
				echo $child->nodeName . '$' . $child->nodeValue . '#';
		}

		echo '_NDBEND_';
	}

	return 'NONE';
}

?>