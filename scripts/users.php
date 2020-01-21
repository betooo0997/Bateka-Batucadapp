<?php

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
	// Echo own user data
	foreach ($user_data as $key => $value)
		echo $key . '$' . $user_data[$key]->nodeValue . '#';

	echo '%';

	// Echo other user data
	$userNodes = $xpath->query("/users/user");

	foreach ($userNodes as $userNode)
	{
		foreach($userNode->childNodes as $child)
			echo $child->nodeName . '$' . $child->nodeValue . '#';

		echo '%';
	}

	return 'NONE';
}

?>