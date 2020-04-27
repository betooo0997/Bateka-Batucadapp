<?php

function modify_empty($obj)
{
	foreach ($obj as $key => $value)
		if($obj->$key == '')
			$obj->$key = 'empty';

	return $obj;
}

function utils_get_xpath($xml_filepath)
{
	$xmlDoc = new DOMDocument();
	$xmlDoc->preserveWhiteSpace = false;
	$success = $xmlDoc->load($xml_filepath);

	$xpath = new DomXpath($xmlDoc);

	if ($success) return [$xmlDoc, $xpath];
	else return [false, false];
}

function set_lock($lock_path, $status)
{
	$file_w = fopen($lock_path, 'w');
	fwrite ($file_w, $status);
	fclose($file_w);
}

function return_error($lock_path, $message, $unlock = true)
{
	if($unlock)
		set_lock($lock_path, '0');

	return $message;
}

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

function scan_directory($path, $minLength, $maxLength, $excludeFile = '')
{
	$files_output = [];
	$files = scandir($path, SCANDIR_SORT_DESCENDING);

	$max_files = 10;
	if (isset($_POST['max_files']))
		$max_files = intval($_POST['max_files']);

	foreach ($files as $file)
	{
		if (strlen($file) >= $minLength && strlen($file) <= $maxLength && $file != $excludeFile)
		{
			$files_output[] = $file;

			if (count($files_output) >= $max_files)
				break;
		}
	}

	return $files_output;
}

?>