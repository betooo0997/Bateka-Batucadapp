<?php

function utils_get_xpath($xml_filepath)
{
	$xmlDoc = new DOMDocument();
	$xmlDoc->preserveWhiteSpace = false;
	$success = $xmlDoc->load($xml_filepath);

	$xpath = new DomXpath($xmlDoc);

	if ($success) return $xpath;
	else return false;
}


?>