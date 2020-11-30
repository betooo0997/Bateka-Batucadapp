<?php

// Redirecting users on login based on user role.
add_filter( 'login_redirect', 'wpdocs_my_login_redirect', 10, 3 );
function wpdocs_my_login_redirect( $url, $request, $user )
{
    if ( $user && is_object( $user ) && is_a( $user, 'WP_User' ) ) 
	{
        if ( $user->has_cap( 'administrator' ) )
            $url = admin_url();
        else
            $url = home_url();//'/en/home';//
    }
    return $url;
}

add_filter( 'login_headerurl', 'wpb_login_logo_url' );
function wpb_login_logo_url() 
{
    return wp_login_url();
}
 
add_filter( 'login_title', 'custom_login_title' );
function custom_login_title( $login_title ) 
{
	return str_replace(array( ' &lsaquo;', ' &#8212; WordPress'), array( ' &bull;', ' Datenbank'),$login_title );
}

add_filter ('allow_password_reset', 'wps_disable_password_reset');
function wps_disable_password_reset() 
{
	return false;
}

add_filter( 'wp_login_errors', 'my_logout_message' );
function my_logout_message($errors)
{
	$logout_message = 'Sie haben sich erfolgreich abgemeldet.'; //'Logged out successfully.';

    if ( isset( $errors->errors['loggedout'] ) )
        $errors->errors['loggedout'][0] = $logout_message;

    return $errors;
}

// Modify login style.
add_action( 'login_enqueue_scripts', 'wpb_login_css' );
function wpb_login_css() 
{ 
	?>
    <style type="text/css">
		body.login div#login p#backtoblog 
		{
			display: none;
		}

		body.login div#login p#nav 
		{
			display: none;
		}

		body.login 
		{
			background-image: url('https://zerales.com/wp-content/uploads/2020/06/Zeralesblau-1.jpg');
			background-repeat: no-repeat;
			background-attachment: fixed;
			background-position: center;
			background-size: cover;
		}

        #login h1 a, .login h1 a 
		{
			background-image: url(https://zerales.com/wp-content/uploads/2020/06/ZeralesLogo_white-1.png);
			height:100px;
			width:300px;
			background-size: 300px 111px;
			background-repeat: no-repeat;
			padding-bottom: 10px;
        }

		#wp-submit, #wp-submit:hover
		{
			background-color: #021349;
			border-color: #555555;
		}

		#wp-submit:visited
		{
			box-shadow: 0 0 1pt 1pt #021349 !important;
		}

		button
		{
			color: #021349 !important;
		}

		#user_pass:focus, #user_login:focus, #rememberme:focus
		{
			box-shadow: 0 0 1pt 1pt #021349;
			border-color: #999999
		}
    </style>
	<?php 
}

add_filter(  'gettext',  'register_text'  );
function register_text( $translating ) 
{
    $translated = str_ireplace(  'Benutzername oder E-Mail-Adresse',  'Benutzername',  $translating );
    //$translated = str_ireplace(  'Benutzername oder E-Mail-Adresse',  'Username',  $translating );
	//$translated = str_ireplace(  'Passwort',  'Password',  $translated );
	//$translated = str_ireplace(  'Angemeldet bleiben',  'Remain signed in',  $translated );
	//$translated = str_ireplace(  'Anmelden',  'Sign in',  $translated );
    return $translated;
}

add_filter('login_errors','login_error_message');
function login_error_message($error)
{
    $error = "<b>Fehler:</b> nicht existierender Benutzername oder falsches Password."; //"Wrong username or password.";
    return $error;
}