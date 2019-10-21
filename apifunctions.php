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

    switch($_POST['REQUEST_TYPE'])
    {
        case 'login':
            handle_login($xpath);
            return;
	}
}

function handle_login($xpath)
{
	$userNode = $xpath->query("/users/user[username='" . $_POST['username'] . "']");

	if ($userNode->length > 0)
		$userNode = $userNode->item(0);
	else
	{
		echo "LOGIN_FAILED_NO_USER.";
		return;
	}

	foreach($userNode->childNodes as $child)
    {
        if($child->nodeName == "psswd")
            $psswdNode = $child;
    }

	if ($psswdNode->nodeValue == $_POST['psswd'])
		echo "LOGIN_SUCCESS.";
	else
		echo "LOGIN_FAILED_WRONG_PSSWD.";
}


?>