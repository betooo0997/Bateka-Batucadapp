<?php

function set_event_vote($con)
{
	// $_POST['event_id'] = 0;
	// $_POST['event_response'] = 2;

	$query = "SELECT events_data FROM users WHERE id = '" . $_POST['id'] . "';";
	$result = mysqli_query($con, $query);
	$object = mysqli_fetch_object($result);
	$events_data = $object->events_data;

	$elements = explode("|", $events_data);
	$updated = false;

	for ($x = 0; $x < sizeof($elements); $x++)
	{
		$tokens = explode("-", $elements[$x]);

		if($tokens[0] == $_POST['event_id'])
		{
			$tokens[1] = $_POST['event_response'];
			$elements[$x] = implode("-", $tokens);
			$updated = true;
			break;
		}
	}

	if ($updated)
		$events_data = implode("|", $elements);
	else
	{
		if (strlen($events_data) > 0)
			$events_data .= "|";

		$events_data .= $_POST['event_id'] . "-" . $_POST['event_response'];
	}

	$query = "UPDATE users SET events_data = '" . $events_data . "' WHERE id = '" . $_POST['id'] . "';";

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating event vote: " . mysqli_error($con);
}

function set_event($con)
{
	/*$_POST['event_name'] = "Ensayo";
	$_POST['event_details'] = "Detalles de ensayo";
	$_POST['event_location_event'] = "Agrupa";
	$_POST['event_location_meeting'] = "Agrupa";
	$_POST['event_date_event'] = "2020-05-30";
	$_POST['event_date_meeting'] = "2020-05-29";
	$_POST['event_date_deadline'] = "2020-05-28";
	$_POST['event_author_id'] = "0";
	$_POST['event_privacy'] = "0";

	$_POST['event_id'] = 1;*/
	$query = "SELECT * FROM events WHERE id = '" . $_POST['event_id'] . "';";
	$result = mysqli_query($con, $query);

	if ($_POST['event_id'] == "0" || mysqli_num_rows($result) == 0)
	{
		$query = "SELECT MAX(id) as max_id FROM events";
		$result = mysqli_query($con, $query);
		$id = intval($result->fetch_object()->max_id) + 1;

		$query = "INSERT INTO events (id, name, details, location_event, location_meeting, date_event, date_meeting, date_deadline, author_id, privacy, transportation, cash, food)
				  VALUES ('" . 
						$id . "', '" .
						$_POST['event_name'] . "', '" .
						$_POST['event_details'] . "', '" .
						$_POST['event_location_event'] . "', '" .
						$_POST['event_location_meeting'] . "', '" .
						$_POST['event_date_event'] . "', '" .
						$_POST['event_date_meeting'] . "', '" .
						$_POST['event_date_deadline'] . "', '" .
						$_POST['event_author_id'] . "', '" .
						$_POST['event_privacy'] . "', '" .
						$_POST['event_transportation'] . "', '" .
						$_POST['event_cash'] . "', '" .
						$_POST['event_food'] .
				  "')";
	}
	else
	{
		$query = "UPDATE events SET
				  name = '" .				$_POST['event_name'] . "', 
				  details = '" .			$_POST['event_details'] . "', 
				  location_event = '" .		$_POST['event_location_event'] . "', 
				  location_meeting = '" .	$_POST['event_location_meeting'] . "', 
				  date_event = '" .			$_POST['event_date_event'] . "', 
				  date_meeting = '" .		$_POST['event_date_meeting'] . "', 
				  date_deadline = '" .		$_POST['event_date_deadline'] . "', 
				  author_id = '" .			$_POST['event_author_id'] . "', 
				  transportation = '" .		$_POST['event_transportation'] . "', 
				  cash = '" .				$_POST['event_cash'] . "', 
				  food = '" .				$_POST['event_food'] . "', 
				  privacy = '" .			$_POST['event_privacy'] . "'
				  WHERE id = '" . $_POST['event_id'] . "'";
	}

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating events: " . mysqli_error($con);
}

?>