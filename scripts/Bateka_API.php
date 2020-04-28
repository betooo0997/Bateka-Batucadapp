<?php
/*
Template Name: Bateka API
*/

?>
<?php
/**
 * The template for displaying all pages.
 *
 * @package Storefront
 */
    if(isset($_POST['API_USER']) && isset($_POST['API_PASSWORD']))
    {
        if($_POST['API_USER'] == 'USER' && $_POST['API_PASSWORD'] == '8420b25f4c1ad7ac906364ee943a7bef')
        {
			try 
			{
				require "wp-content/asambleapp/batekapp/apifunctions.php";
				$internal_error = handleRequest();
			} 
			catch (Exception $e) 
			{
				echo '*ERROR 500*',  $e->getMessage(), "\n";
				return;
			}

			if ($internal_error != 'NONE') 
				echo '*ERROR 500*' . $internal_error;
        }
        else
        {
            echo'LOGIN NOT AUTHORIZED. THIS ACTION WILL BE REPORTED.';
        }
    }
    else
    {
        echo 'LOGIN NOT AUTHORIZED.';		
    }
?>