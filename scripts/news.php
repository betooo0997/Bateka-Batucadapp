<?php

function set_news_entry($con)
{
	/*$_POST['news_name'] = "Noticia";
	$_POST['news_details'] = "Detalles de la notica";
	$_POST['news_date_creation'] = "2020-05-29";
	$_POST['news_author_id'] = "1";
	$_POST['news_privacy'] = "2";
	$_POST['news_images'] = "www.image3.jpg";

	$_POST['news_id'] = 0;*/
	$query = "SELECT * FROM news WHERE id = '" . $_POST['news_id'] . "';";
	$result = mysqli_query($con, $query);

	if ($_POST['news_id'] == "0" || mysqli_num_rows($result) == 0)
	{
		$query = "SELECT MAX(id) as max_id FROM news";
		$result = mysqli_query($con, $query);
		$id = intval($result->fetch_object()->max_id) + 1;

		$query = "INSERT INTO news (id, name, details, date_creation, author_id, privacy, images)
				  VALUES ('" . 
						$id . "', '" .
						$_POST['news_name'] . "', '" .
						$_POST['news_details'] . "', '" .
						$_POST['news_date_creation'] . "', '" .
						$_POST['news_author_id'] . "', '" .
						$_POST['news_privacy'] . "', '" .
						$_POST['news_images'] .  
				  "')";
	}
	else
	{
		$query = "UPDATE news SET
				  name = '" .				$_POST['news_name'] . "', 
				  details = '" .			$_POST['news_details'] . "', 
				  date_creation = '" .		$_POST['news_date_creation'] . "', 
				  author_id = '" .			$_POST['news_author_id'] . "', 
				  privacy = '" .			$_POST['news_privacy'] . "',
				  images = '" .				$_POST['news_images'] . "'
				  WHERE id = '" . $_POST['news_id'] . "'";
	}

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating news: " . mysqli_error($con);
}

function set_news_entry_seen($con)
{
	$query = "SELECT news_data FROM users WHERE id = '" . $_POST['id'] . "';";
	$result = mysqli_query($con, $query);
	$object = mysqli_fetch_object($result);
	$news_data = $object->news_data;

	$elements = explode("|", $news_data);
	$present = false;

	for ($x = 0; $x < sizeof($elements); $x++)
	{
		if($elements[$x] == $_POST['news_id'])
		{
			$present = true;
			break;
		}
	}

	if(!$present)
	{
		if (strlen($news_data) > 0)
			$news_data .= "|";

		$news_data .= $_POST['news_id'];
		$query = "UPDATE users SET news_data = '" . $news_data . "' WHERE id = '" . $_POST['id'] . "';";

		if (mysqli_query($con, $query))
			return "NONE";
		else
			return "Error updating news_entry: " . mysqli_error($con);
	}

	return "NONE";
}

?>