<?php

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


?>