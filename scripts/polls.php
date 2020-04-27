<?php

function get_poll_data($con)
{
	$query = "SELECT * FROM polls;";
	$result = mysqli_query($con, $query);

    while ($obj = mysqli_fetch_object($result))
	{
		$m_obj = modify_empty($obj);
		foreach ($m_obj as $key => $value)
			echo $m_obj->$key . '#';
		echo '%';
	}

	return 'NONE';
}

function set_poll_vote($con)
{
	// $_POST['poll_id'] = 1;
	// $_POST['poll_response'] = 0;

	$query = "SELECT polls_data FROM users WHERE id = '" . $_POST['id'] . "';";
	$result = mysqli_query($con, $query);
	$object = mysqli_fetch_object($result);
	$polls_data = $object->polls_data;

	$elements = explode("|", $polls_data);
	$updated = false;

	for ($x = 0; $x < sizeof($elements); $x++)
	{
		$tokens = explode("-", $elements[$x]);

		if($tokens[0] == $_POST['poll_id'])
		{
			$tokens[1] = $_POST['poll_response'];
			$elements[$x] = implode("-", $tokens);
			$updated = true;
			break;
		}
	}

	if ($updated)
		$polls_data = implode("|", $elements);
	else
		$polls_data .= $_POST['poll_id'] . "-" . $_POST['poll_response'];

	$query = "UPDATE users SET polls_data = '" . $polls_data . "' WHERE id = '" . $_POST['id'] . "';";

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating poll vote: " . mysqli_error($con);
}

function set_poll($con)
{
	/*$_POST['poll_name'] = "Encuesta";
	$_POST['poll_details'] = "Detalles de la encuesta";
	$_POST['poll_date_deadline'] = "2020-05-30";
	$_POST['poll_date_creation'] = "2020-05-29";
	$_POST['poll_author_id'] = "0";
	$_POST['poll_privacy'] = "0";
	$_POST['poll_options'] = "no|si";

	$_POST['poll_id'] = 1;*/
	$query = "SELECT * FROM polls WHERE id = '" . $_POST['poll_id'] . "';";
	$result = mysqli_query($con, $query);

	if (mysqli_num_rows($result) == 0)
	{
		$query = "INSERT INTO polls (id, name, details, date_creation, date_deadline, author_id, privacy, options)
				  VALUES ('" . 
						$_POST['poll_id'] . "', '" .
						$_POST['poll_name'] . "', '" .
						$_POST['poll_details'] . "', '" .
						$_POST['poll_date_creation'] . "', '" .
						$_POST['poll_date_deadline'] . "', '" .
						$_POST['poll_author_id'] . "', '" .
						$_POST['poll_privacy'] . "', '" .
						$_POST['poll_options'] .  
				  "')";
	}
	else
	{
		$query = "UPDATE polls SET
				  name = '" .				$_POST['poll_name'] . "', 
				  details = '" .			$_POST['poll_details'] . "', 
				  date_creation = '" .		$_POST['poll_date_creation'] . "', 
				  date_deadline = '" .		$_POST['poll_date_deadline'] . "', 
				  author_id = '" .			$_POST['poll_author_id'] . "', 
				  privacy = '" .			$_POST['poll_privacy'] . "',
				  options = '" .			$_POST['poll_options'] . "'
				  WHERE id = '" . $_POST['poll_id'] . "'";
	}

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating polls: " . mysqli_error($con);
}

?>