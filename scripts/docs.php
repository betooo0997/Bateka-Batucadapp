<?php

function set_doc($con)
{
	/*$_POST['doc_name'] = "Documento";
	$_POST['doc_details'] = "Detalles del documento";
	$_POST['doc_date_creation'] = "2020-05-29";
	$_POST['doc_author_id'] = "0";
	$_POST['doc_privacy'] = "1";
	$_POST['doc_imgs'] = "";
	$_POST['doc_urls'] = "www.url.jpg";

	$_POST['doc_id'] = 0;*/
	$query = "SELECT * FROM docs WHERE id = '" . $_POST['doc_id'] . "';";
	$result = mysqli_query($con, $query);

	if (mysqli_num_rows($result) == 0)
	{
		$query = "SELECT MAX(id) as max_id FROM polls";
		$result = mysqli_query($con, $query);
		$id = intval($result->fetch_object()->max_id) + 1;

		$query = "INSERT INTO docs (id, name, details, date_creation, author_id, privacy, imgs, urls)
				  VALUES ('" . 
						$id . "', '" .
						$_POST['doc_name'] . "', '" .
						$_POST['doc_details'] . "', '" .
						$_POST['doc_date_creation'] . "', '" .
						$_POST['doc_author_id'] . "', '" .
						$_POST['doc_privacy'] . "', '" .
						$_POST['doc_imgs'] . "', '" .
						$_POST['doc_urls'] .  
				  "')";
	}
	else
	{
		$query = "UPDATE docs SET
				  name = '" .				$_POST['doc_name'] . "', 
				  details = '" .			$_POST['doc_details'] . "', 
				  date_creation = '" .		$_POST['doc_date_creation'] . "', 
				  author_id = '" .			$_POST['doc_author_id'] . "', 
				  privacy = '" .			$_POST['doc_privacy'] . "',
				  imgs = '" .				$_POST['doc_imgs'] . "',
				  urls = '" .				$_POST['doc_urls'] . "'
				  WHERE id = '" . $_POST['doc_id'] . "'";
	}

	if (mysqli_query($con, $query))
		return "NONE";
	else
		return "Error updating docs: " . mysqli_error($con);
}

?>