<?php

add_shortcode('activation_codes', 'add_activation_codes');

function get_code_names()
{
	$buttons = array();

	if(Get_Lang() == 'de')
	{
		$buttons[] = 'Einweisung - Schritt 3';
		$buttons[] = 'G&auml;steliste';
		$buttons[] = 'Luxa Catering';
		$buttons[] = 'Caterham Industries';
		$buttons[] = 'Gespr&auml;ch Empfangsmann';
		$buttons[] = 'Besuch F&uuml;hrungsabteilung';
		$buttons[] = 'Obduktion';
		$buttons[] = 'Verh&ouml;r A. Hildebrandt';
		$buttons[] = 'Druck A. Hildebrandt';
		$buttons[] = 'Marastra&szlig;e';
		$buttons[] = 'Tiefgr&uuml;ndige Untersuchung';
		$buttons[] = 'Tatort';
		$buttons[] = 'Hausdurchsuchung';
		$buttons[] = '&Uuml;bersetzung Artikel';
		$buttons[] = 'Wanduhrfach';
		$buttons[] = '&Uuml;berwachungskameras';
		$buttons[] = 'Verh&ouml;r L. Erfurt';
		$buttons[] = 'Druck L. Erfurt';
		$buttons[] = 'Verh&ouml;r K. Siedendorf';
		$buttons[] = 'Aufzeichnung Zug&auml;nge Westfl&uuml;gel';
		$buttons[] = 'Aufzeichnung Garten Westfl&uuml;gel';
		$buttons[] = 'Aufzeichnung R&auml;ume Ostfl&uuml;gel';
		$buttons[] = 'Aufzeichnung Verbindungsgang';
		$buttons[] = 'Aufzeichnung Zug&auml;nge Ostfl&uuml;gel';
		$buttons[] = 'Aufzeichnung Garten Ostfl&uuml;gel';
	}
	else
	{
		$buttons[] = 'Briefing - Step 3';
		$buttons[] = 'Guest list';
		$buttons[] = 'Luxa Catering';
		$buttons[] = 'Caterham Industries';
		$buttons[] = 'Receptionist';
		$buttons[] = 'Management department';
		$buttons[] = 'Autopsy';
		$buttons[] = 'Questioning A. Hildebrandt';
		$buttons[] = 'Pressure A. Hildebrandt';
		$buttons[] = 'Marastreet';
		$buttons[] = 'Deep Examination';
		$buttons[] = 'Crime scene';
		$buttons[] = 'House search';
		$buttons[] = 'Translation article';
		$buttons[] = 'Wall clock compartment';
		$buttons[] = 'Video footage';
		$buttons[] = 'Questioning L. Erfurt';
		$buttons[] = 'Pressure L. Erfurt';
		$buttons[] = 'Questioning K. Siedendorf';
		$buttons[] = 'Recordings entrance west wing';
		$buttons[] = 'Recordings garden west wing';
		$buttons[] = 'Recordings rooms east wing';
		$buttons[] = 'Recordings connecting corridor';
		$buttons[] = 'Recordings entrance east wing';
		$buttons[] = 'Recordings garden east wing';
	}
	return $buttons;
}

function get_codes()
{
	$buttons = array();
	$buttons[] = '7565';
	$buttons[] = '8375';
	$buttons[] = '0294';
	$buttons[] = '1940';
	$buttons[] = '7391';
	$buttons[] = '1905';
	$buttons[] = '3446';
	$buttons[] = '8216';
	$buttons[] = '4273';
	$buttons[] = '8827';
	$buttons[] = '3394';
	$buttons[] = '1847';
	$buttons[] = '9124';
	$buttons[] = '2841';
	$buttons[] = '0381';
	$buttons[] = '6129';
	$buttons[] = '4561';
	$buttons[] = '5124';
	$buttons[] = '4002';
	$buttons[] = '0723';
	$buttons[] = '3589';
	$buttons[] = '1329';
	$buttons[] = '1494';
	$buttons[] = '6503';
	$buttons[] = '2948';
	return $buttons;
}

function get_dependencies()
{
	$buttons = array();
	$buttons[] = array();
	$buttons[] = array();
	$buttons[] = array(1);
	$buttons[] = array(1);
	$buttons[] = array(3);
	$buttons[] = array(3);
	$buttons[] = array();
	$buttons[] = array(6);
	$buttons[] = array(7);
	$buttons[] = array(8);
	$buttons[] = array(6);
	$buttons[] = array();
	$buttons[] = array(11);
	$buttons[] = array(12);
	$buttons[] = array(12);
	$buttons[] = array(11);
	$buttons[] = array(22);
	$buttons[] = array(16);
	$buttons[] = array(22);
	$buttons[] = array(15);
	$buttons[] = array(15);
	$buttons[] = array(15);
	$buttons[] = array(15);
	$buttons[] = array(15);	
	$buttons[] = array(15);

	return $buttons;
}

function add_activation_codes($hide = false, $id = 0, $msg = '') 
{
	$user_id = wp_get_current_user()->ID;
	$user_meta = get_user_meta($user_id);

	$codes_per_day = intval(get_meta($user_meta, 'codes_per_day', "20"));
	$used_codes = intval(get_meta($user_meta, 'used_codes_daily', '0'));

	$buttons = get_code_names();
	$codes = get_codes();
	$dependencies = get_dependencies();

	$post_name = $buttons[intval(preg_replace('/\D/', '', $_POST['code_id']))];
	$content_to_add = '';
	$content = '';

	if(!$hide)
		$content = '<div id="code_content" style="overflow:auto">';

	$codes_left_label = 'Verbleibende Codes';
	$codes_link = 'aktivierungscodes';

	if(Get_Lang() == 'en')
	{
		$codes_left_label = 'Remaining Codes';
		$codes_link = 'en/activation-codes';
	}

	$content .= '
		<div style="width:100%; text-align:center; padding-top: 25px; font-size: large;">
			' . $codes_left_label . ': ' . ($codes_per_day - $used_codes) . '
		</div>';

	$content .='
		<div class="full_width_small_screen" style="float:left; width:350px; margin-top:10px;">
			<h5 style="margin-bottom:0px; font-size:x-large;"><b>Codes</b></h5>
			 <form action="/' . $codes_link . '" method="post">';
	
	$after_0 = '';
	$after_1 = '';

	foreach ($buttons as $key => $name)
	{
		$key_used = 'activation_code_' . $key;

		$enabled = true;

		foreach ($dependencies[$key] as $element)
		{
			if (!isset($user_meta['activation_code_' . $element]))
				$enabled = false;
		}

		if($id == $key_used)
		{
			if(isset($user_meta[$key_used]))
				$after_0 .= '<button id="trigger" class="activate_code_disabled" type="submit" disabled>' . $name . ':<div style="float:right">' . $codes[$key] . '</div></button>';
			else
				$after_0 .= '<button class="activate_code" name="code_id" type="submit" value="' . $key_used . '">' . $name . '</button>';
			$after_0 .= $msg;
		}
		else if(isset($user_meta[$key_used]) || $key_used == $_POST['code_id'] && isset($_POST['code_confirmed']))
			$content .= '<button class="activate_code_disabled" type="submit" disabled>' . $name . ':<div style="float:right">' . $codes[$key] . '</div></button>';
		else if ($enabled)
			$after_1 .= '<button class="activate_code" name="code_id" type="submit" value="' . $key_used . '">' . $name . '</button>';
		//else
			//$after_2 .= '<button class="activate_code_disabled" type="submit" disabled>' . $name . '<div style="float:right">Gesperrt</div></button>';
	}

	$content .= $after_0 . $after_1 . '
			</form>
		</div>';

	if($content_to_add == '')
		$add_class = 'class="show_if_small_screen"';

	$content .= '
		<div class="not_show_if_big_screen ' . $add_class . '" style="float:left; width:100%; margin-right:50px; text-align:left !important">' 
			. add_history(true) . 
		'</div>';

	$content .= '
	</div>';

	if(!$hide)
		$content .= '</div>';
		
	return $content;
}

//Insertar Javascript js y enviar ruta admin-ajax.php
add_action('wp_enqueue_scripts', 'dcms_insert_js');

function dcms_insert_js()
{
    wp_register_script('dcms_confirm_panel', '/wp-content/plugins/theme-customisations/custom/activation_codes_ajax.js', array('jquery'), '1', true );
    wp_enqueue_script('dcms_confirm_panel');
    wp_localize_script('dcms_confirm_panel','dcms_vars',['ajaxurl'=>admin_url('admin-ajax.php')]);

	wp_register_script('dcms_reset_codes', '/wp-content/plugins/theme-customisations/custom/reset_activation_codes_ajax.js', array('jquery'), '1', true );
    wp_enqueue_script('dcms_reset_codes');
    wp_localize_script('dcms_reset_codes','dcms_vars',['ajaxurl'=>admin_url('admin-ajax.php')]);
}

add_action('wp_ajax_nopriv_dcms_ajax_activation_codes','dcms_send_confirm_panel');
add_action('wp_ajax_dcms_ajax_activation_codes','dcms_send_confirm_panel');

function dcms_send_confirm_panel()
{
	$codes_left_label = array('Code unter dem Namen', ' anfordern');
	$codes_link = 'aktivierungscodes';
	$confirm_label = 'Ja';
	$cancel_label = 'Abbruch';

	if(Get_Lang() == 'en')
	{
		$codes_left_label = array('Request code', '');
		$codes_link = 'en/activation-codes';
		$confirm_label = 'Confirm';
		$cancel_label = 'Cancel';
	}

	echo '	
		<div id="message" class="question_code">
			<div>
				' . $codes_left_label[0] . ' <b>' . $_POST['description'] . '</b>'. $codes_left_label[1] . '?<br/>
			</div>
			<form action="/' . $codes_link . '" method="post">
				<input type="hidden" id="code_id" value="' . $_POST['code_id'] . '" />
				<button style="width:150px;" id="confirm_code" name="code_confirmed" type="submit" value="Y">' . $confirm_label . '</button>
				<button style="width:150px;" id="abort_confirm" name="code_confirmed" type="submit" value="N">' . $cancel_label . '</button>
			</form>
		</div>';

	wp_die();
}

add_action('wp_ajax_nopriv_dcms_ajax_activation_codes_confirm','dcms_send_confirmed_code');
add_action('wp_ajax_dcms_ajax_activation_codes_confirm','dcms_send_confirmed_code');

function dcms_send_confirmed_code()
{
	$user_id = wp_get_current_user()->ID;
	$user_meta = get_user_meta($user_id);

	$codes_per_day = intval(get_meta($user_meta, 'codes_per_day', "20"));
	$used_codes_total = intval(get_meta($user_meta, 'used_codes_total', '0'));
	$used_codes_daily = intval(get_meta($user_meta, 'used_codes_daily', '0'));

	$codes_link = 'aktivierungscodes';
	$code_unlocked = 'freigeschaltet';
	$code_excess = 'Sie haben leider die Anzahl der anforderbaren Aktivierungscodes &uuml;berschritten.';
	$code_already_unlocked = 'Dieser code wurde schon freigeschaltet!';

	if(Get_Lang() == 'en')
	{
		$codes_link = 'en/activation-codes';
		$code_unlocked = 'has been unlocked';
		$code_excess = 'Unfortunately, you have exceeded the number of activation codes that can be requested.';
		$code_already_unlocked = 'This code has already been unlocked!';
	}

	$message = '<div id="message" class="question_code">';

	$id = $_POST['code_id'];
	$clean_id = str_replace('activation_code_', '', $id);
	$code = get_codes()[$clean_id];

	if(!array_key_exists($id, $user_meta) || $user_meta[$id][0] != 'true')
	{
		if($used_codes_daily < $codes_per_day)
		{
			$used_codes_daily++;
			$used_codes_total++;
			update_user_meta($user_id, 'used_codes_daily', strval($used_codes_daily));
			update_user_meta($user_id, 'used_codes_total', strval($used_codes_total));
			update_user_meta($user_id, $id, 'true');
			$message .= 'Code <b>' . $post_name . '</b> ' . $code_unlocked . '!';
		}
		else
		{
			$message .= $code_excess;
		}
	}
	else
	{
		$message .= $code_already_unlocked . ' Code: ' . $code;
	}

	$message .= '</div>';

	echo add_activation_codes(true, $id, $message);

	$hour = intval($used_codes_total * 0.5 + 8.5);
	$minute = '00';
	$half = (($used_codes_total + 1) % 2 == 1);

	if($half)
		$minute = '30';

	if($hour < 10)
		$hour = '0' . strval($hour);
	else
		$hour = strval($hour);

	echo '%spl%' . $hour . '%spl%' . $minute ;

	wp_die();
}

add_shortcode('reset_activationcodes', 'show_reset_codes_button');

function show_reset_codes_button()
{
	return '<button id="reset_codes" value="' . um_profile_id() .'" class="um-button">Aktivierungscodes zur&uuml;cksetzen</button>';
}

add_action('wp_ajax_nopriv_dcms_ajax_dcms_reset_codes','dcms_send_reset_codes');
add_action('wp_ajax_dcms_ajax_dcms_reset_codes','dcms_send_reset_codes');

function dcms_send_reset_codes()
{
	$user_id = $_POST['user_id'];
	$user_meta = get_user_meta($user_id);

	$deleted = 0;

	for($x = 0; $x < 26; $x++)
	{
		if(array_key_exists('activation_code_' . $x, $user_meta))
		{
			delete_user_meta($user_id, 'activation_code_' . $x);
			$deleted += 1;
		}
	}

	delete_user_meta($user_id, 'used_codes_daily');
	delete_user_meta($user_id, 'used_codes_total');

	echo '<br/>Es wurden ' . $deleted . ' Aktivierungscodes zur&uuml;ckgesetzt.';

	wp_die();
}