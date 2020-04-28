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

	if (mysqli_num_rows($result) == 0)
	{
		$query = "INSERT INTO news (id, name, details, date_creation, author_id, privacy, images)
				  VALUES ('" . 
						$_POST['news_id'] . "', '" .
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

?>