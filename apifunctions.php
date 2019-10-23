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
    $databasePath = "wp-content/bateka_batukadapp/database/user_database.xml";

    $xmlDoc = new DOMDocument();
    $xmlDoc->load($databasePath);
    $xpath = new DomXpath($xmlDoc);

	$user_data = get_user_nodes($xpath);

	if ($user_data != false && $user_data["psswd"]->nodeValue == $_POST['psswd'])
	{
		switch($_POST['REQUEST_TYPE'])
		{
			case 'login':
				handle_login($user_data);
				break;
			case 'get_users':
				handle_get_users($xpath);
				return;
		}
	}
	else echo "WRONG_CREDENTIALS.";
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

function handle_login($user_data)
{
	echo "LOGIN_SUCCESS.";
	echo "|";

	foreach ($user_data as $key => $value)
		echo $key . "$" . $user_data[$key]->nodeValue . "#";
}

function handle_get_users($xpath)
{
}


?>