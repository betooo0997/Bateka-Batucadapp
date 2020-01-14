<?php

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
    $databasePath = "wp-content/asambleapp/database/user_database.xml";

    $xmlDoc = new DOMDocument();
    $success = $xmlDoc->load($databasePath);

	if ($success)
	{
		$xpath = new DomXpath($xmlDoc);

		$user_data = get_user_nodes($xpath);

		if ($user_data != false && $user_data["psswd"]->nodeValue == $_POST['psswd'])
		{
			echo "VERIFIED.|";

			switch($_POST['REQUEST_TYPE'])
			{
				case 'get_data':
					get_user_data($user_data, $xpath);
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
}


?>