<?php

function get_user_data($con)
{
	function query($changer)
	{
		return "SELECT id, username, name, surname, role, telephone, email, polls_data, events_data 
				FROM users 
				WHERE id " . $changer . "= '" . $_POST['id'] . "';";
	}

	// Echo own user data
	$result = mysqli_query($con, query(""));
	$obj = mysqli_fetch_object($result);
	$obj = modify_empty($obj);

	foreach ($obj as $key => $value)
		echo $obj->$key . '#';

	echo '%';

	// Echo other user data
	$result = mysqli_query($con, query("!"));

    while ($obj = mysqli_fetch_object($result))
	{
		$m_obj = modify_empty($obj);
		foreach ($m_obj as $key => $value)
			echo $m_obj->$key . '#';
		echo '%';
	}

	return 'NONE';
}

?>