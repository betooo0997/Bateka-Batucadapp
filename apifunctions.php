<?php

include "wp-content/asambleapp/utils.php";

function sanitize_xml($databasePath)
{
    $xmlcontent = htmlentities(file_get_contents($databasePath));

    $tokens = preg_split( "/(\?xml|\/users)/", $xmlcontent);

    $content = html_entity_decode('<?xml' . $tokens[1] . '/users>');

    $databaseFile = fopen($databasePath, 'w') or die("Unable to open file!");
    fwrite($databaseFile, $content);
    fclose($databaseFile);

    return $content;
}

function handleRequest()
{	
    $databasePath = "wp-content/asambleapp/batekapp/database/user_database.xml";
	$xpath = utils_get_xpath($databasePath);

	if ($xpath != false)
	{
		$user_data = get_user_nodes($xpath);

		if ($user_data != false && $user_data["psswd"]->nodeValue == $_POST['psswd'])
		{
			echo "VERIFIED.|";

			switch($_POST['REQUEST_TYPE'])
			{
				case 'get_data':
					return get_user_data($user_data, $xpath);
					break;

				case 'get_polls':
					return get_poll_data();
					break;

				default:
					echo 'REQUEST_TYPE' . $_POST['REQUEST_TYPE'] . 'not understood';
					break;
			}
		}
		else echo "WRONG_CREDENTIALS.";
		return "NONE";
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
	// Get own user data
	foreach ($user_data as $key => $value)
		echo $key . "$" . $user_data[$key]->nodeValue . "#";

	echo "%";

	// Get other user data
	$userNodes = $xpath->query("/users/user");

	foreach ($userNodes as $userNode)
	{
		foreach($userNode->childNodes as $child)
			echo $child->nodeName . "$" . $child->nodeValue . "#";

		echo "%";
	}

	return "NONE";
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
		$xpath = utils_get_xpath($pollDatabasePath);

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
					foreach ($comment->childNodes as $data)
						echo $data->nodeName . '^' . $data->nodeValue . '~';

				echo '#';
			}
		}
	}

	echo '|';
	return "NONE";
}


?>