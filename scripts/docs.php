<?php

function get_docs_data()
{
	$files_output = [];
	$files = scandir("wp-content/asambleapp/batekapp/database/docs");

	$max_files = 5;
	if (isset($_POST['max_files']))
		$max_files = intval($_POST['max_files']);

	foreach ($files as $file)
	{
		if (strlen($file) >= 9 && strlen($file) <= 11)
		{
			$files_output[] = $file;

			if (count($files_output) >= $max_files)
				break;
		}
	}

	foreach ($files_output as $file)
	{
		$database_path = "wp-content/asambleapp/batekapp/database/docs/" . $file;
		$xpath = utils_get_xpath($database_path)[1];

		if ($xpath == false)
			return "Couldn't load docs database '" . $file . "'.";

		$rootNode = $xpath->query("/doc")[0];

		foreach ($rootNode->childNodes as $child)
		{
			if($child->nodeName == 'imgs')
			{
				echo 'imgs$';
				foreach ($child->childNodes as $grandchild)
					echo $grandchild->nodeValue . '~';
				echo '#';
			}
			else if ($child->nodeName == 'content_urls')
			{
				echo 'content_urls$';
				foreach ($child->childNodes as $grandchild)
					echo $grandchild->nodeValue . '~';
				echo '#';
			}
			else
				echo $child->nodeName . '$' . $child->nodeValue . '#';
		}

		echo '_DBEND_';
	}

	return 'NONE';
}

?>