<?php

function set_rhythm($con)
{
	/*$_POST['rhythm_name'] = "Ritmo";
	$_POST['rhythm_details'] = "Detalles del ritmo";
	$_POST['rhythm_date_update'] = "2020-05-30";
	$_POST['rhythm_date_creation'] = "2020-05-29";
	$_POST['rhythm_author_id'] = "1";
	$_POST['rhythm_data'] = "{}";

	$_POST['rhythm_id'] = 0;*/
	$query = "SELECT * FROM rhythms WHERE id = '" . $_POST['rhythm_id'] . "';";
	$result = mysqli_query($con, $query);

	if ($_POST['rhythm_id'] == "0" || mysqli_num_rows($result) == 0)
	{
		$query = "SELECT MAX(id) as max_id FROM rhythms";
		$result = mysqli_query($con, $query);
		$id = intval($result->fetch_object()->max_id) + 1;

		$query = "INSERT INTO rhythms (id, name, details, ppm, time_signature, date_creation, date_update, author_id, data)
				  VALUES ('" . 
						$id . "', '" .
						$_POST['rhythm_name'] . "', '" .
						$_POST['rhythm_details'] . "', '" .
						$_POST['rhythm_ppm'] . "', '" .
						$_POST['rhythm_time_signature'] . "', '" .
						$_POST['rhythm_date_creation'] . "', '" .
						$_POST['rhythm_date_update'] . "', '" .
						$_POST['rhythm_author_id'] . "', '" .
						$_POST['rhythm_data'] .  
				  "')";
	}
	else
	{
		$query = "UPDATE rhythms SET
				  name = '" .				$_POST['rhythm_name'] . "', 
				  details = '" .			$_POST['rhythm_details'] . "', 
				  ppm = '" .				$_POST['rhythm_ppm'] . "', 
				  time_signature = '" .		$_POST['time_signature'] . "', 
				  date_creation = '" .		$_POST['rhythm_date_creation'] . "', 
				  date_update = '" .		$_POST['rhythm_date_update'] . "', 
				  author_id = '" .			$_POST['rhythm_author_id'] . "', 
				  data = '" .				$_POST['rhythm_data'] . "'
				  WHERE id = '" . $_POST['rhythm_id'] . "'";
	}

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating rhythms: " . mysqli_error($con);
}

?>