<?php

function get_rhythms_data()
{
	$files_output = scan_directory("wp-content/asambleapp/batekapp/database/rhythms", 12, 14);

	foreach ($files_output as $file)
	{
		$database_path = "wp-content/asambleapp/batekapp/database/rhythms/" . $file;
		$xpath = utils_get_xpath($database_path)[1];

		if ($xpath == false)
			return "Couldn't load rhythm database '" . $file . "'.";

		$rootNode = $xpath->query("/rhythm")[0];

		foreach ($rootNode->childNodes as $child)
		{
			if($child->nodeName == 'sound')
			{
				echo 'sound$';
				foreach ($child->childNodes as $grandchild)
				{
					if ($grandchild->nodeName == 'type')
					{
						echo $grandchild->nodeValue . '~';
						break;
					}
				}

				foreach ($child->childNodes as $grandchild)
					if ($grandchild->nodeName != 'type')
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